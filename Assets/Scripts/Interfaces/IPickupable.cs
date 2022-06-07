using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    /// <summary>
    /// Hung on objects that can be picked up
    /// </summary>
    public interface IPickupable
    {
        ///<param name="holdParent">Player hands</param>
        void PickUp(Transform holdParent);

        /// <param name="positionForPutObject">Place to put the object</param>
        void Drop(Vector3 positionForPutObject);
    }
}