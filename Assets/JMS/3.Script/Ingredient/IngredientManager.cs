using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cooking
{
    /// <summary>
    /// 재료가 조리된 정도
    /// </summary>
    public enum RipeState
    {
        Raw = 0,
        Undercook = 1,
        Welldone = 2,
        Overcook = 3,
        Burnt = 4,
    }

    public enum CookType
    {
        None = 0,
        Boil = 1,
        Grill = 2,
    }

    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(MeshCalculator))]
    public class IngredientManager : MonoBehaviour
    {
        

        [field: SerializeField] public float RipeValue { get; private set; } = 0f;
        public float m_volumeWeight = 1f; // private으로 바꾸세요~

        [Tooltip("재료 정보")]
        public IngredientSO data;

        [Header("Cook State")]
        [Tooltip("잘린 조각이면 false, 전체이면 true")]
        public bool isWhole = true;
        [field: Tooltip("현재 조리방식")]
        [field: SerializeField] public CookType CookType { get; set; } = CookType.None;
        [field: Tooltip("현재 익음상태")]
        [field: SerializeField] public RipeState RipeState { get; private set; } = RipeState.Raw;

        private Renderer m_renderer;
        private MeshCalculator m_meshCalculator;

        private void Awake()
        {
            TryGetComponent(out m_renderer);
            TryGetComponent(out m_meshCalculator);

            if (data == null)
                Debug.LogError($"There is no ingredient data. [ObjectName : {gameObject.name}]");
        }

        private void Start()
        {
            m_volumeWeight = data.weightOverVolume.Evaluate(m_meshCalculator.Volume);
            m_renderer.material = data.rawMaterial;
        }

        private void Update()
        {
            CheckCookState();

            // Check volume
            if (!m_meshCalculator.isUpdating) return;
            m_volumeWeight = data.weightOverVolume.Evaluate(m_meshCalculator.Volume);
        }
        private void CheckCookState()
        {
            if (RipeState == RipeState.Burnt) return;

            if (RipeState == RipeState.Raw
                && RipeValue < 200f && RipeValue >= 100f)
            {
                RipeState = RipeState.Undercook;
                m_renderer.material = data.undercookMaterial;
            }
            else if (RipeState == RipeState.Undercook
                     && RipeValue < 300f && RipeValue >= 200f)
            {
                RipeState = RipeState.Welldone;
                m_renderer.material = data.welldoneMaterial;
            }
            else if (RipeState == RipeState.Welldone
                     && RipeValue < 400f && RipeValue >= 300f)
            {
                RipeState = RipeState.Overcook;
                m_renderer.material = data.overcookMaterial;
            }

            else if (RipeState == RipeState.Overcook
                     && RipeValue < 500f && RipeValue >= 400f)
            {
                RipeState = RipeState.Burnt;
                m_renderer.material = data.burntMaterial;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (RipeState == RipeState.Burnt
                || other.gameObject.layer != LayerMask.NameToLayer("Boil")
                && other.gameObject.layer != LayerMask.NameToLayer("Grill")) return;

            if (other.gameObject.layer == LayerMask.NameToLayer("Boil"))
            {
                CookType = CookType.Boil;
                return;
            }
                
            if (other.gameObject.layer == LayerMask.NameToLayer("Grill"))
            {
                CookType = CookType.Grill;
                return;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (RipeState == RipeState.Burnt
                || other.gameObject.layer != LayerMask.NameToLayer("Boil")
                && other.gameObject.layer != LayerMask.NameToLayer("Grill")) return;

            // 삶기
            if (other.gameObject.TryGetComponent(out BoilerManager boiler))
            {
                RipeValue += GetWeight() * boiler.RipePerSecond * Time.deltaTime;
                return;
            }

            // 굽기
            if (other.gameObject.TryGetComponent(out GrillManager griller))
            {
                RipeValue += GetWeight() * griller.RipePerSecond * Time.deltaTime;
                return;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (RipeState == RipeState.Burnt
                || other.gameObject.layer != LayerMask.NameToLayer("Boil")
                && other.gameObject.layer != LayerMask.NameToLayer("Grill")) return;

            CookType = CookType.None;
        }

        private float GetWeight() => data.baseWeight * m_volumeWeight;
    }
}
