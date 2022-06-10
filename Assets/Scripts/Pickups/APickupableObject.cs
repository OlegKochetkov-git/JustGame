using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    /// <summary>
    /// Class responsible for the item that can be picked up
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Collider))]
    public abstract class APickupableObject : MonoBehaviour, IPickupable
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

        public virtual void Drop(Vector3 positionForPutObject)
        {
            rb.useGravity = true;
            rb.drag = 1f;
            rb.freezeRotation = false;
            transformGO.parent = null;
            colliderGO.enabled = true;
            transformGO.position = positionForPutObject;
        }
        #endregion
    }
}
