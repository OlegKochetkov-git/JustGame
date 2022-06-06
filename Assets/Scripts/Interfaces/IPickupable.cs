using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Interfaces
{
    public interface IPickupable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="holdParent">Place where to move the object</param>
        void PickUp(Transform holdParent); 
    }
}