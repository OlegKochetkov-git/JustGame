using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Assets.Player.Scripts
{
    /// <summary>
    /// The class responsible for launching the ray
    /// </summary>
    public abstract class ARayCastingObjects : MonoBehaviour
    {
        private float rayLenghtForDebug = 15f;

        protected Transform cameraTransform;
        protected Ray ray;
        protected RaycastHit hit;
        protected int ingnoreLayerMask; // don't forget about ~

        protected void Awake()
        {
            cameraTransform = Camera.main.transform;

            ingnoreLayerMask = 1 << LayerMask.NameToLayer("Player");
        }

        protected abstract void InputHandleForLaunchRay();

        /// <summary>Ray ignoring player layer</summary>
        /// <param name="rayLength"></param>
        /// <returns>
        /// Object that the ray collided with, or null.
        /// </returns>
        protected GameObject ShotRay(float rayLength)
        {
            ray = new Ray(cameraTransform.position, cameraTransform.forward);
            bool hitSomething = Physics.Raycast(ray, out hit, rayLength, ~ingnoreLayerMask);
            if (hitSomething)
            {
                GameObject hitableObject = hit.collider.gameObject;
                return hitableObject;
            }
            else
            {
                //Debug.LogWarning($"Ray hit nothing");
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

