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

        private void Update()
        {
            InputHandleForLaunchRay();
        }

        protected override void InputHandleForLaunchRay()
        {
            if (!Keyboard.current.eKey.wasPressedThisFrame) return;

            GameObject hitableObject = ShotRay(rayLength);
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