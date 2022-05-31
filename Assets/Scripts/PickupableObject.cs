using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    public class PickupableObject : MonoBehaviour, IPickupable
    {
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        #region Interfaces
        public void PickUp(Transform holdParent)
        {
            rb.useGravity = false;
            rb.drag = 10f;
            gameObject.transform.parent = holdParent;
        }
        #endregion
    }
}
