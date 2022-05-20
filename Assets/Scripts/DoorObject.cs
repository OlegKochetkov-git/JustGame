using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorObject : OpenableObject
{
    public override void InteractWithObject()
    {
        if (isAnimated) return;

        if (transformGO.localEulerAngles == atStartLocalAngle)
            StartCoroutine(Open(true));
        else
            StartCoroutine(Open(false));
    }

    protected override IEnumerator Open(bool isOpen)
    {
        return base.Open(isOpen);
    }
}