using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cooking
{
    /// <summary>
    /// 재료가 조리된 정도
    /// </summary>
    public enum CookState
    {
        Raw = 0,
        Under = 1,
        Well = 2,
        Over = 3,
        Burnt = 4,
    }

    [RequireComponent(typeof(MeshCalculator))]
    public class IngredientManager : MonoBehaviour
    {
        public float RipeValue { get; set; } = 0f;
        public float _volumeWeight = 1f;

        [Tooltip("재료 정보")]
        public IngredientSO data;

        [Header("Cook State")]
        [Tooltip("잘린 조각이면 false, 전체이면 true")]
        public bool isWhole = true;
        [field:Tooltip("현재 조리상태")]
        [field: SerializeField] public CookState CookState { get; private set; } = CookState.Raw;

        MeshCalculator _meshCalculator;

        private void Awake()
        {
            TryGetComponent(out _meshCalculator);

            if (data == null)
                Debug.LogError($"There is no ingredient data. [ObjectName : {gameObject.name}]");
        }

        private void Start()
        {
            _volumeWeight = data.weightOverVolume.Evaluate(_meshCalculator.Volume);
        }

        private void Update()
        {
            if (!_meshCalculator.isUpdating) return;

            _volumeWeight = data.weightOverVolume.Evaluate(_meshCalculator.Volume);
        }
    }
}
