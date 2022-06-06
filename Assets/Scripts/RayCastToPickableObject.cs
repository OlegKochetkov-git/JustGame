using Assets.Player.Scripts;
using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class RayCastToPickableObject : RayCastingObjects
    {
        [SerializeField] private Transform handsTransform;
        [SerializeField] private float rayLength;
        [SerializeField] private LayerMask layerMask;

        private GameObject objectInHand;

        private void Update()
        {
            InputHandleForLaunchRay();
            MoveObject();
        }

        protected override void InputHandleForLaunchRay()
        {
            if (!Keyboard.current.eKey.wasPressedThisFrame) return;

            GameObject hitableObject = ShotRay(rayLength, layerMask.value);
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
                if (hitableObject.layer == 10)
                {
                    DropObject(hitableObject.layer);
                }
            }

        }

        void DropObject(int a)
        {
            Rigidbody rb = objectInHand.GetComponent<Rigidbody>();
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            RaycastHit hit;
            bool hitSomething = Physics.Raycast(ray, out hit, 15f, 1024);
            rb.useGravity = true;
            rb.drag = 1f;
            rb.freezeRotation = false;
            rb.transform.parent = null;
            objectInHand = null;
      
            if (hitSomething)
            {
                Debug.Log("HIT");
                Vector3 pos = hit.point;
                rb.gameObject.transform.position = pos + Vector3.up;
                rb.gameObject.GetComponent<Collider>().enabled = true;

            }
            
        }

        private void MoveObject()
        {
            if (objectInHand == null) return;
            objectInHand.transform.position = handsTransform.position;
        }

       
    }


}