using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cooking
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(MeshCalculator))]
    [RequireComponent(typeof(Rigidbody))]
    public class IngredientManager : MonoBehaviour
    {
        [field: SerializeField] public float Ripe { get; private set; } = 0f;
        private float m_volumeWeight = 1f;

        [Tooltip("��� ����")]
        public IngredientSO data;

        [Header("Current State")]
        [Tooltip("�߸� �����̸� false, ��ü�̸� true")]
        public bool isWhole = true;
        [field: Tooltip("���� �������")]
        [field: SerializeField] public CookType CookType { get; set; } = CookType.None;
        [field: Tooltip("���� ��������")]
        [field: SerializeField] public RipeState RipeState { get; private set; } = RipeState.Raw;

        [Header("Cook History")]
        [SerializeField] public float m_ripeByBoil = 0f;
        [SerializeField] public float m_ripeByBroil = 0f;
        [SerializeField] public float m_ripeByGrill = 0f;
        public float RateOfBoil => Ripe == 0 ? 0f : m_ripeByBoil / Ripe * 100f;
        public float RateOfBroil => Ripe == 0 ? 0f : m_ripeByBroil / Ripe * 100f;
        public float RateOfGrill => Ripe == 0 ? 0f : m_ripeByGrill / Ripe * 100f;

        private Renderer m_renderer;
        private MeshCalculator m_meshCalculator;
        private Rigidbody m_rigidbody;

        private void Awake()
        {
            TryGetComponent(out m_renderer);
            TryGetComponent(out m_meshCalculator);
            TryGetComponent(out m_rigidbody);
        }

        private void Start()
        {
            if (data == null)
			{
                Debug.LogWarning($"There is no IngredientSO data. [ObjectName : {gameObject.name}]");
                return;
			}

            m_volumeWeight = data.weightOverVolume.Evaluate(m_meshCalculator.Volume);
            m_renderer.material = data.rawMaterial;
        }

        private void Update()
        {
            if (data == null)
            {
                return;
            }

            CheckCookState();

            // Check volume
            if (!m_meshCalculator.isUpdating) return;
            m_volumeWeight = data.weightOverVolume.Evaluate(m_meshCalculator.Volume);
        }

        private void CheckCookState()
        {
            if (RipeState == RipeState.Burn) return;

            if (RipeState == RipeState.Raw
                && data.ripeForWelldone < 200f && Ripe >= data.ripeForUndercook)
            {
                RipeState = RipeState.Undercook;
                m_renderer.material = data.undercookMaterial;
            }
            else if (RipeState == RipeState.Undercook
                     && Ripe < data.ripeForOvercook && Ripe >= data.ripeForWelldone)
            {
                RipeState = RipeState.Welldone;
                m_renderer.material = data.welldoneMaterial;
            }
            else if (RipeState == RipeState.Welldone
                     && Ripe < data.ripeForBurn && Ripe >= data.ripeForOvercook)
            {
                RipeState = RipeState.Overcook;
                m_renderer.material = data.overcookMaterial;
            }

            else if (RipeState == RipeState.Overcook
                     && Ripe >= data.ripeForBurn)
            {
                RipeState = RipeState.Burn;
                m_renderer.material = data.burnMaterial;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var otherObj = other.gameObject;

            // ���� �ȿ� �� ���
            if (otherObj.TryGetComponent(out DishManager dishManager))
            {
                transform.SetParent(otherObj.transform);
            }
            
            // �丮 ������ ���
            if (!IsCookable(otherObj, out CookerManager manager)) return;
            if (manager is BoilManager)
            {
                CookType = CookType.Boil;
                return;
            }
            else if (manager is BroilManager)
            {
                CookType = CookType.Broil;
                return;
            }
            else if (manager is GrillManager)
            {
                CookType = CookType.Grill;
                return;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            var otherObj = other.gameObject;
            if (!IsCookable(otherObj, out CookerManager manager)) return;

            // Ripe
            var ripeValue = GetWeight() * manager.RipePerSecond * Time.deltaTime;

            if (manager is BoilManager)
            {
                m_ripeByBoil += ripeValue;
            }
            else if (manager is BroilManager)
            {
                m_ripeByBroil += ripeValue;
            }
            else if (manager is GrillManager)
            {
                m_ripeByGrill += ripeValue;
            }

            Ripe += ripeValue;
        }

        private void OnTriggerExit(Collider other)
        {
            var otherObj = other.gameObject;

            // ���� �ȿ��� ���� ���
            if (otherObj.TryGetComponent(out DishManager dishManager))
            {
                transform.SetParent(null);
            }

            // �丮 �ߴܵ� ���
            if (!IsCookable(otherObj, out _)) return;
            CookType = CookType.None;
        }

        /// <summary>
        /// return base weight * volume weight
        /// </summary>
        /// <returns>total weight</returns>
        private float GetWeight() => data.baseWeight * m_volumeWeight;

        private bool IsCookable(GameObject obj, out CookerManager manager) {
            manager = null;

            if (RipeState != RipeState.Burn
                && obj.TryGetComponent(out CookerManager cookerManager))
                manager = cookerManager;

            return manager != null;
        }

        public IEnumerator FlyToDish(Transform dish, float maxOffsetY, float endOffsetY, float randomRange, float duration, AnimationCurve positionOverTime)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = dish.position + new Vector3(Random.Range(-randomRange, randomRange),
                                                        endOffsetY,
                                                        Random.Range(-randomRange, randomRange));
            Vector3 midPos = new Vector3(
                (startPos.x + endPos.x) / 2,
                endPos.y + maxOffsetY,
                (startPos.z + endPos.z) / 2);

            // isKinematic = true, ��θ� �̵��ϸ� �߻��ϴ� ������ �ذ�
            // 1. ����� ������ ������ �߷��� ����Ǿ� ������ ����
            // 2. �ٸ� Object��� �ǵ����� ���� �浹
            m_rigidbody.isKinematic = true;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                yield return null;
                elapsedTime += Time.deltaTime;

                float progress = positionOverTime.Evaluate(elapsedTime / duration);
                transform.position = GetPointOnBezierCurve3(startPos, midPos, endPos, progress);
            }

            m_rigidbody.isKinematic = false;
        }
        private Vector3 GetPointOnBezierCurve3(Vector3 startPos, Vector3 midPos, Vector3 endPos, float progress)
        {
            var p1 = Vector3.Lerp(startPos, midPos, progress);
            var p2 = Vector3.Lerp(midPos, endPos, progress);

            return Vector3.Lerp(p1, p2, progress);
        }
    }
}
