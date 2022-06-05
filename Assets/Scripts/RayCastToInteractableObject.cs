using Assets.Player.Scripts;
using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class RayCastToInteractableObject : RayCastingObjects
    {
        [SerializeField] private float rayLength;
        [SerializeField] private LayerMask layerMask;

        private void Update()
        {
            InputHandleForLaunchRay();
        }

        protected override void InputHandleForLaunchRay()
        {
            if (!Keyboard.current.eKey.wasPressedThisFrame) return;

            GameObject hitableObject = ShotRay(rayLength, layerMask.value);
            if (hitableObject == null) return;

            ActionWithHittableObject(hitableObject);
        }

        private void ActionWithHittableObject(GameObject hitObject)
        {
            if (!hitObject.GetComponent(typeof(IInteractable))) return;

            hitObject.GetComponent<IInteractable>().InteractWithObject();
        }
    }
}