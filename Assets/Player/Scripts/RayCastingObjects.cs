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
        [SerializeField] float rayLenghtForDebug;

        private Transform cameraTransform;
        private Ray ray;
        private RaycastHit hit;

        private void Awake()
        {
            cameraTransform = Camera.main.transform;
        }

        /// <summary>
        /// Returns the object that the ray collided with, or null.
        /// </summary>
        /// <param name="rayLength"></param>
        /// <returns></returns>
        public GameObject ShotRay(float rayLength)
        {
            ray = new Ray(cameraTransform.position, cameraTransform.forward);
            bool hitSomething = Physics.Raycast(ray, out hit, rayLength);
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

