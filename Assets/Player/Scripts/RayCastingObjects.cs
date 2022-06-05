using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Assets.Player.Scripts
{
    public abstract class RayCastingObjects : MonoBehaviour
    {
        [SerializeField] private float rayLenghtForDebug;

        private Transform cameraTransform;
        private Ray ray;
        private RaycastHit hit;

        protected void Awake()
        {
            cameraTransform = Camera.main.transform;
        }

        protected abstract void InputHandleForLaunchRay();

        /// <param name="rayLength"></param>
        /// <param name="currentLayerMask">The mask we want to face</param>
        /// <returns>
        /// Object that the ray collided with, or null.
        /// </returns>
        protected GameObject ShotRay(float rayLength, int currentLayerMask)
        {
            ray = new Ray(cameraTransform.position, cameraTransform.forward);
            bool hitSomething = Physics.Raycast(ray, out hit, rayLength, currentLayerMask);
            if (hitSomething)
            {
                GameObject hitableObject = hit.collider.gameObject;
                return hitableObject;
            }
            else
            {
                Debug.LogWarning($"Ray hit nothing");
                return null;
            }
        }
       
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            if (cameraTransform == null) return;
            Gizmos.DrawRay(cameraTransform.transform.position, cameraTransform.forward * rayLenghtForDebug);
        }
    }
}

