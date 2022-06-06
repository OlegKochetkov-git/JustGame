using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public abstract class PickupableObject : MonoBehaviour, IPickupable
    {
        protected Transform transformGO;
        protected Rigidbody rb;
        protected Collider colliderGO;

        protected void Awake()
        {
            transformGO = GetComponent<Transform>();
            rb = GetComponent<Rigidbody>();
            colliderGO = GetComponent<Collider>();
        }

        #region Interfaces
        public virtual void PickUp(Transform holdParent)
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
