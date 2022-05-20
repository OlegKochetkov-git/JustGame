using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class OpenableObject : MonoBehaviour, IInteractable
{
    public Vector3 angleOfRotation;
    public Vector3 movementLength;
    public float timeForMovingDoor;

    protected Transform transformGO;
    protected Vector3 atStartLocalAngle;
    protected Vector3 currAngleRotation;

    protected Vector3 atStartLocalPosition;
    protected Vector3 currLocalPosition;

    protected bool isAnimated;

    private void Awake()
    {
        transformGO = GetComponent<Transform>();

        atStartLocalPosition = transformGO.localPosition;
        currLocalPosition = transformGO.localPosition;

        atStartLocalAngle = transformGO.localEulerAngles;
        currAngleRotation = transformGO.localEulerAngles;
    }
    #region Interfaces
    public virtual void InteractWithObject() { }
    #endregion

    protected virtual IEnumerator Open(bool isOpen)
    {
        isAnimated = true;

        Vector3 startPosition = currLocalPosition;
        Vector3 endPosition = startPosition + movementLength * ChooseDirection(isOpen);

        Vector3 startRotation = currAngleRotation;
        Vector3 endRotation = startRotation + angleOfRotation * ChooseDirection(isOpen);

        float t = 0f;
        while (t < timeForMovingDoor)
        {
            t += Time.deltaTime;
            float xMovement = Mathf.Lerp(startPosition.x, endPosition.x, t / timeForMovingDoor) % 360f;
            float yMovement = Mathf.Lerp(startPosition.y, endPosition.y, t / timeForMovingDoor) % 360f;
            float zMovement = Mathf.Lerp(startPosition.z, endPosition.z, t / timeForMovingDoor) % 360f;

            float xRotation = Mathf.Lerp(startRotation.x, endRotation.x, t / timeForMovingDoor) % 360f;
            float yRotation = Mathf.Lerp(startRotation.y, endRotation.y, t / timeForMovingDoor) % 360f;
            float zRotation = Mathf.Lerp(startRotation.z, endRotation.z, t / timeForMovingDoor) % 360f;

            transformGO.localPosition = new Vector3(xMovement, yMovement, zMovement);
            transformGO.localEulerAngles = new Vector3(xRotation, yRotation, zRotation);

            currLocalPosition = transformGO.localPosition;
            currAngleRotation = transformGO.localEulerAngles;

            yield return null;
        }

        isAnimated = false;
    }

    protected int ChooseDirection(bool isOpen)
    {
        int openingDoor = 1;
        if (!isOpen)
            openingDoor = -1;

        return openingDoor;
    }
}
