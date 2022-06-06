using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Assets.Scripts
{
    public class TableBox : AFurnitureObject
    {
        #region Interfaces
        public override void InteractWithObject()
        {
            if (isAnimated) return;

            if (transformGO.localPosition == atStartLocalPosition)
                StartCoroutine(Open(true));
            else
                StartCoroutine(Open(false));
        }
        #endregion

        protected override IEnumerator Open(bool isOpen)
        {
            return base.Open(isOpen);
        }
    }
}
