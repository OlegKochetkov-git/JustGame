using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IPickupable
    {
        /// <param name="holdParent">Place where to move the object</param>
        void PickUp(Transform holdParent); 
        void Drop(Vector3 positionForPutObject);
    }
}