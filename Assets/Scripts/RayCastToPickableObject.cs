using Assets.Player.Scripts;
using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class RayCastToPickableObject : ARayCastingObjects
    {
        [SerializeField] private Transform handsTransform;
        [SerializeField] private float rayLength;

        private GameObject objectInHand;

        private void Update()
        {
            InputHandleForLaunchRay();
            MoveObject();
        }

        protected override void InputHandleForLaunchRay()
        {
            if (!Keyboard.current.eKey.wasPressedThisFrame) return;

            GameObject hitableObject = ShotRay(rayLength);
            if (hitableObject == null) return;

            ActionWithHittableObject(hitableObject);
        }


        private void ActionWithHittableObject(GameObject hitableObject)
        {
            if (objectInHand == null)
                TryPickupObjectInHand(hitableObject);          
            else
                TryDropObject(hitableObject);
        }

        private void TryPickupObjectInHand(GameObject hitableObject)
        {
            bool isAvailableForPickUp = hitableObject.GetComponent(typeof(IPickupable));
            if (!isAvailableForPickUp) return;

            objectInHand = hitableObject;
            IPickupable objectForPickUp = objectInHand.GetComponent<IPickupable>();
            objectForPickUp.PickUp(handsTransform);
        }

        private void TryDropObject(GameObject hitableObject)
        {
            //bool isAvailableForDrop = hitableObject.GetComponent(typeof(IPickupable));
            //if (!isAvailableForDrop) return;

            if (!hitableObject.CompareTag("Put")) return; // better use interface

            IPickupable objectForDrop = objectInHand.GetComponent<IPickupable>();
            objectForDrop.Drop(hit.point);
            objectInHand = null;
        }

        private void MoveObject()
        {
            if (objectInHand == null) return;
            objectInHand.transform.position = handsTransform.position;
        }
    }
}