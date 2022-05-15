using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Furniture
{  
    Door,
    TableBox
}

[RequireComponent(typeof(Collider))]
public class InteractbleObject : MonoBehaviour, IInteractable
{
    [SerializeField] Furniture furniture;

    [Header("Door Settings:")]
    [SerializeField] Vector3 angleOfRotarion;

    [Header("TableBox Settings:")]
    [SerializeField] Vector3 movementLength;

    Transform transformGO;
    Vector3 atStartLocalAngle;
    Vector3 atStarLocalTableBoxPosition;

    private void Awake()
    {
        transformGO = GetComponent<Transform>();

        atStartLocalAngle = transformGO.localRotation.eulerAngles;
        atStarLocalTableBoxPosition = transformGO.localPosition;
    }

    public void InteractWithObject()
    {
        if (furniture == Furniture.Door)
            InteractiveWithDoor();
        
        if (furniture == Furniture.TableBox)
            InteractiveWithTableBox();
    }

    private void InteractiveWithDoor()
    {
        if (transformGO.localRotation.eulerAngles == atStartLocalAngle)
            transformGO.localRotation = Quaternion.Euler(angleOfRotarion);
        else
            transformGO.localRotation = Quaternion.Euler(atStartLocalAngle);
    }

    private void InteractiveWithTableBox()
    {
        if (atStarLocalTableBoxPosition == transformGO.localPosition)
            transformGO.localPosition += movementLength;
        else
            transformGO.localPosition -= movementLength;
    }
}