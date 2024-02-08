using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

    
namespace WCH
{
    public class Vr_UIMover : MonoBehaviour
    {
        public ActionBasedController xrController;
        public XRRayInteractor xrRayInteractor;
        private GameObject hoverUI;
        private Transform hoverUITransform;

        private float hitDistance;
        private float hitAngle;

        private bool isHover = false;
        private bool isFirstGrip = false;

        private bool isCoroutine = false;

        private void Awake()
        {
            xrController = GetComponentInParent<ActionBasedController>();
            TryGetComponent(out xrRayInteractor);
        }
        public void UIHoverEntered(UIHoverEventArgs e)
        {
            if (e.uiObject.CompareTag("HoverUI"))
            {
                hoverUI = e.uiObject;
                hoverUITransform = hoverUI.transform.root.GetComponent<Transform>();
                isHover = true;
            }
        }

        public void UIHoverExited(UIHoverEventArgs e)
        {
            if (e.uiObject.CompareTag("HoverUI") && !xrController.selectAction.action.IsPressed())
            {
                hoverUI = null;
                isHover = false;
                isFirstGrip = false;
            }
        }

        private void Update()
        {
            if (!isHover) return;


            if (!isFirstGrip)
            {
                isFirstGrip = true;
                hitDistance = Vector3.Distance(transform.position, hoverUITransform.position);
                hitAngle = Vector3.Angle(transform.forward, hoverUITransform.forward);
                Debug.Log(hitDistance);
            }
            if (xrController.selectAction.action.IsPressed())
            {
                Debug.Log("is trigger btn");
                Vector3 position = (transform.forward * hitDistance) + transform.position;

                hoverUITransform.position = position;
                hoverUITransform.rotation = Quaternion.RotateTowards(transform.rotation, hoverUITransform.rotation, hitAngle);
            }
            else if (!xrController.selectAction.action.IsPressed())
            {
                isFirstGrip = false;
            }
        }

        public void SortRotation()
        {
            if (isCoroutine) return;

            float rotaionY = hoverUITransform.rotation.eulerAngles.y;
            //hoverUITransform.rotation = Quaternion.Euler(0, rotaionY, 0);
            StartCoroutine(SmoothRotate(hoverUITransform, Quaternion.Euler(0, rotaionY, 0), 0.5f));
        }

        private IEnumerator SmoothRotate(Transform origin, Quaternion target, float time)
        {
            isCoroutine = true;
            float elapsedTime = 0f;
            while (elapsedTime < time)
            {
                Quaternion rotation = Quaternion.Slerp(origin.rotation, target, elapsedTime / time);
                origin.rotation = rotation;
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            origin.rotation = target;
            isCoroutine = false;
            yield break;
        }
    }
}
