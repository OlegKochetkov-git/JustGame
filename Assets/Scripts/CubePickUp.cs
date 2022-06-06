using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    public class CubePickUp : APickupableObject
    {
        #region Interfaces
        public override void PickUp(Transform holdParent)
        {
            base.PickUp(holdParent);
        }

        public override void Drop(Vector3 positionForPutObject)
        {
            base.Drop(positionForPutObject + (Vector3.up / 2f));
        }
        #endregion
    }
}

