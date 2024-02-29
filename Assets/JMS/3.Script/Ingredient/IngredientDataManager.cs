using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Cooking
{
    [RequireComponent(typeof(MeshCalculator))]
    public class IngredientDataManager : MonoBehaviour
    {
        [field: SerializeField] public float Ripe { get; private set; } = 0f;
        private float _volumeWeight = 1f;

        [Tooltip("재료 정보")]
        public IngredientSO data;

        [Header("Current State")]
        [Tooltip("잘린 조각이면 false, 안잘렸으면 true")]
        public bool isWhole = true;
        [field: Tooltip("현재 조리방식")]
        [field: SerializeField] public CookType CookType { get; set; } = CookType.None;
        [field: Tooltip("현재 익음상태")]
        [field: SerializeField] public RipeState RipeState { get; private set; } = RipeState.Raw;

        [Header("Cook History")]
        [SerializeField] public float _ripeByBoil = 0f;
        [SerializeField] public float _ripeByBroil = 0f;
        [SerializeField] public float _ripeByGrill = 0f;
        public float RateOfBoil => Ripe == 0 ? 0f : _ripeByBoil / Ripe * 100f;
        public float RateOfBroil => Ripe == 0 ? 0f : _ripeByBroil / Ripe * 100f;
        public float RateOfGrill => Ripe == 0 ? 0f : _ripeByGrill / Ripe * 100f;

        private MeshCalculator _meshCalculator;

        [Header("Object")]
        public Renderer _renderer;
        public Rigidbody _rigidbody;

        private void Awake()
        {
            TryGetComponent(out _meshCalculator);
            
            Assert.IsNotNull(_meshCalculator, $"Can not find MeshCalculator component in {gameObject.name}");
            Assert.IsNotNull(_renderer, $"Can not find Renderer component for {gameObject.name}");
            Assert.IsNotNull(_rigidbody, $"Can not find Rigidbody component for {gameObject.name}");
            //Assert.IsNotNull(data, $"There is no IngredientSO data for {gameObject.name}");
        }

        private void Start()
        {
            _volumeWeight = data.weightOverVolume.Evaluate(_meshCalculator.Volume);
            _renderer.material = data.rawMaterial;
        }

        private void Update()
        {
            if (data == null)
            {
                return;
            }

            CheckCookState();

            // Check volume
            if (!_meshCalculator.isUpdating) return;
            _volumeWeight = data.weightOverVolume.Evaluate(_meshCalculator.Volume);
        }

        private void CheckCookState()
        {
            if (RipeState == RipeState.Burn) return;

            if (RipeState == RipeState.Raw
                && data.ripeForUndercook <= Ripe)
            {
                RipeState = RipeState.Undercook;
                _renderer.material = data.undercookMaterial;
            }
            else if (RipeState == RipeState.Undercook
                      && data.ripeForWelldone <= Ripe && Ripe < data.ripeForOvercook)
            {
                RipeState = RipeState.Welldone;
                _renderer.material = data.welldoneMaterial;
            }
            else if (RipeState == RipeState.Welldone
                      && data.ripeForOvercook <= Ripe && Ripe < data.ripeForBurn)
            {
                RipeState = RipeState.Overcook;
                _renderer.material = data.overcookMaterial;
            }
            else if (RipeState == RipeState.Overcook
                     && data.ripeForBurn <= Ripe)
            {
                RipeState = RipeState.Burn;
                _renderer.material = data.burnMaterial;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            var otherObj = other.gameObject;

            // 접시 안에 들어간 경우
            if (otherObj.TryGetComponent(out DishManager dishManager))
            {
                transform.SetParent(otherObj.transform);
            }
            
            // 요리 시작한 경우
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
                _ripeByBoil += ripeValue;
            }
            else if (manager is BroilManager)
            {
                _ripeByBroil += ripeValue;
            }
            else if (manager is GrillManager)
            {
                _ripeByGrill += ripeValue;
            }

            Ripe += ripeValue;
        }

        private void OnTriggerExit(Collider other)
        {
            var otherObj = other.gameObject;

            // 접시 안에서 나온 경우
            if (otherObj.TryGetComponent(out DishManager dishManager))
            {
                transform.SetParent(null);
            }

            // 요리 중단된 경우
            if (!IsCookable(otherObj, out _)) return;
            CookType = CookType.None;
        }

        /// <summary>
        /// return base weight * volume weight
        /// </summary>
        /// <returns>total weight</returns>
        private float GetWeight() => data.baseWeight/* * _volumeWeight*/;

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

            // isKinematic = true, 경로를 이동하며 발생하는 문제를 해결
            // 1. 경로의 끝에서 누적된 중력이 적용되어 빠르게 낙하
            // 2. 다른 Object들과 의도하지 않은 충돌
            _rigidbody.isKinematic = true;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                yield return null;
                elapsedTime += Time.deltaTime;

                float progress = positionOverTime.Evaluate(elapsedTime / duration);
                transform.position = GetPointOnBezierCurve3(startPos, midPos, endPos, progress);
            }

            _rigidbody.isKinematic = false;
        }
        private Vector3 GetPointOnBezierCurve3(Vector3 startPos, Vector3 midPos, Vector3 endPos, float progress)
        {
            var p1 = Vector3.Lerp(startPos, midPos, progress);
            var p2 = Vector3.Lerp(midPos, endPos, progress);

            return Vector3.Lerp(p1, p2, progress);
        }
    }
}
