using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Assets.Player.Scripts
{
    public class RayCastingObjects : MonoBehaviour
    {
        [SerializeField] Transform handsTransform;
        [SerializeField] float rayLength;

        GameObject objectInHand;
        private Transform cameraTransform;
        private Ray ray;
        private RaycastHit hit;

        private void Awake()
        {
            cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                ray = new Ray(cameraTransform.position, cameraTransform.forward);
                bool hitSomething = Physics.Raycast(ray, out hit, rayLength);
                if (hitSomething)
                {
                    GameObject hitObject = hit.collider.gameObject;

                    ActionsWithInteractiveObject(hitObject);
                    ActionWithPickUpObject(hitObject);
                }
            }

            if (objectInHand != null)
            {
                MoveObject();
            }
        }

        private void ActionWithPickUpObject(GameObject hitObject)
        {
            if (objectInHand == null)
            {
                if (hitObject.GetComponent(typeof(IPickupable)))
                {
                    objectInHand = hitObject;
                    objectInHand.GetComponent<IPickupable>().PickUp(handsTransform);
                }
            }
            else
            {
                DropObject();
            }

        }

        private void ActionsWithInteractiveObject(GameObject hitObject)
        {
            if (!hitObject.GetComponent(typeof(IInteractable))) return;

            hitObject.GetComponent<IInteractable>().InteractWithObject();
            
        }

        void DropObject()
        {
            var rb = objectInHand.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.drag = 1f;
            rb.transform.parent = null;
            objectInHand = null;
        }

        private void MoveObject()
        {
            objectInHand.transform.position = handsTransform.position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (cameraTransform == null) return;
            Gizmos.DrawRay(cameraTransform.transform.position, cameraTransform.forward * rayLength);
        }
    }
}

