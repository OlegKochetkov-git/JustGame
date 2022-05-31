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
    
        private Transform cameraTransform;
        private Ray ray;
        private RaycastHit hit;

        private void Awake()
        {
            cameraTransform = Camera.main.transform;
        }


        public GameObject ShotRay(float rayLength)//, out GameObject hitable)
        {
            ray = new Ray(cameraTransform.position, cameraTransform.forward);
            bool hitSomething = Physics.Raycast(ray, out hit, rayLength);
            if (hitSomething)
            {
                //hitable = hit.collider.gameObject;
                return hit.collider.gameObject;
            }
            else
            {
                //hitable = null;
                return null;
            }
        }

        private void Update()
        {
            //if (Mouse.current.leftButton.wasPressedThisFrame)
            //{
            //    ray = new Ray(cameraTransform.position, cameraTransform.forward);
            //    bool hitSomething = Physics.Raycast(ray, out hit, rayLength);
            //    if (hitSomething)
            //    {
            //        GameObject hitObject = hit.collider.gameObject;

            //        ActionsWithInteractiveObject(hitObject);
            //        ActionWithPickUpObject(hitObject);
            //    }
            //}

            
        }

        

        private void ActionsWithInteractiveObject(GameObject hitObject)
        {
            if (!hitObject.GetComponent(typeof(IInteractable))) return;

            hitObject.GetComponent<IInteractable>().InteractWithObject();
            
        }

        

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (cameraTransform == null) return;
            Gizmos.DrawRay(cameraTransform.transform.position, cameraTransform.forward * 20f);
        }
    }
}

