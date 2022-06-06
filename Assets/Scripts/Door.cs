using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class Door : AFurnitureObject
    {
        #region Interfaces
        public override void InteractWithObject()
        {
            if (isAnimated) return;

            if (transformGO.localEulerAngles == atStartLocalAngle)
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
