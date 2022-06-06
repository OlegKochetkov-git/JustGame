using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PickupableObject : MonoBehaviour, IPickupable
    {
        private Transform transformGO;
        private Rigidbody rb;
        private Collider colliderGO;

        private void Awake()
        {
            transformGO = GetComponent<Transform>();
            rb = GetComponent<Rigidbody>();
            colliderGO = GetComponent<Collider>();
        }

        #region Interfaces
        public void PickUp(Transform holdParent)
        {
            colliderGO.enabled = false; 
            rb.useGravity = false;
            rb.drag = 10f;
            rb.freezeRotation = true;
            transformGO.parent = holdParent;  
        }
        #endregion
    }
}
