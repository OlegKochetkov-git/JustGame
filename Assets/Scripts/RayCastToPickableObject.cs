using Assets.Player.Scripts;
using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class RayCastToPickableObject : MonoBehaviour
    {
        [SerializeField] private Transform handsTransform;
        [SerializeField] private float rayLength;

        private RayCastingObjects rayCastingObjects;
        private GameObject objectInHand;

        private void Awake()
        {
            rayCastingObjects = GetComponent<RayCastingObjects>();
        }

        private void Update()
        {
            InputHandleForLaunchRay();
            MoveObject();
        }

        private void InputHandleForLaunchRay()
        {
            if (!Keyboard.current.eKey.wasPressedThisFrame) return;
            
            GameObject hitableObject = rayCastingObjects.ShotRay(rayLength);
            if (hitableObject == null) return;

            ActionWithHittableObject(hitableObject);
        }

        private void ActionWithHittableObject(GameObject hitableObject)
        {
            if (objectInHand == null)
            {
                bool isAvailableForPickUp = hitableObject.GetComponent(typeof(IPickupable));
                if (isAvailableForPickUp)
                {
                    objectInHand = hitableObject;
                    IPickupable objectForPickUp = objectInHand.GetComponent<IPickupable>();
                    objectForPickUp.PickUp(handsTransform);
                }
            }
            else
            {
                if (hitableObject.GetComponent(typeof(IPickupable)))
                {
                    DropObject();
                }
            }

        }

        void DropObject()
        {
            Rigidbody rb = objectInHand.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.drag = 1f;
            rb.transform.parent = null;
            objectInHand = null;
        }

        private void MoveObject()
        {
            if (objectInHand == null) return;
            objectInHand.transform.position = handsTransform.position;
        }
    }


}