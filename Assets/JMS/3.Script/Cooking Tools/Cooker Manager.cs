using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Cooking
{
    public abstract class CookerManager : MonoBehaviour
    {
        protected Collider cookArea;

        [field: SerializeField]
        [field: Range(0.1f, 10f)] public float RipePerSecond { get; set; } = 1f;

        // Common functions for cooker (boiler / broiler / griller)
        protected virtual void Awake()
        {
            TryGetComponent(out cookArea);
        }

        protected virtual void Start()
        {
            ToggleArea(false);
        }
        public virtual void ToggleArea(bool isOn)
        {
            cookArea.gameObject.SetActive(isOn);
        }

        // XR Grab Interactable Events
        public virtual void OnActivated(ActivateEventArgs e)
        {
            ToggleArea(true);
        }

        // XR Grab Interactable Events
        public virtual void OnDeactivated(DeactivateEventArgs e)
        {
            ToggleArea(false);
        }
    }
}
