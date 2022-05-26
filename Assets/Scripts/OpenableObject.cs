using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Furniture
{
    None,
    Door,
    TableBox
}


[RequireComponent(typeof(Collider))]
public abstract class OpenableObject : MonoBehaviour, IInteractable
{
    public Furniture furniture;

    public Vector3 angleOfRotation;
    public Vector3 movementLength;
    public float timeForMovingDoor;

    protected Transform transformGO;
    protected Vector3 atStartLocalAngle;
    protected Vector3 currAngleRotation;
    protected Vector3 atStartLocalPosition;
    protected Vector3 currLocalPosition;

    protected bool isAnimated;

    #region Interfaces
    public virtual void InteractWithObject() { }
    #endregion

    private void Awake()
    {
        transformGO = GetComponent<Transform>();

        atStartLocalPosition = transformGO.localPosition;
        currLocalPosition = transformGO.localPosition;

        atStartLocalAngle = transformGO.localEulerAngles;
        currAngleRotation = transformGO.localEulerAngles;
    } 

    protected virtual IEnumerator Open(bool isOpen)
    {
        isAnimated = true;

        if (furniture == Furniture.Door)
        {
            Vector3 startRotation = currAngleRotation;
            Vector3 endRotation = startRotation + angleOfRotation * ChooseDirection(isOpen);

            float t = 0f;
            while (t < timeForMovingDoor)
            {
                t += Time.deltaTime;

                float xRotation = Mathf.Lerp(startRotation.x, endRotation.x, t / timeForMovingDoor) % 360f;
                float yRotation = Mathf.Lerp(startRotation.y, endRotation.y, t / timeForMovingDoor) % 360f;
                float zRotation = Mathf.Lerp(startRotation.z, endRotation.z, t / timeForMovingDoor) % 360f;

                transformGO.localEulerAngles = new Vector3(xRotation, yRotation, zRotation);
                currAngleRotation = transformGO.localEulerAngles;

                yield return null;
            }
        }

        if (furniture == Furniture.TableBox)
        {
            Vector3 startPosition = currLocalPosition;
            Vector3 endPosition = startPosition + movementLength * ChooseDirection(isOpen);

            float t = 0f;
            while (t < timeForMovingDoor)
            {
                t += Time.deltaTime;
                float xMovement = Mathf.Lerp(startPosition.x, endPosition.x, t / timeForMovingDoor) % 360f;
                float yMovement = Mathf.Lerp(startPosition.y, endPosition.y, t / timeForMovingDoor) % 360f;
                float zMovement = Mathf.Lerp(startPosition.z, endPosition.z, t / timeForMovingDoor) % 360f;

                transformGO.localPosition = new Vector3(xMovement, yMovement, zMovement);
                currLocalPosition = transformGO.localPosition;

                yield return null;
            }
        }      

        isAnimated = false;
    }

    protected int ChooseDirection(bool isOpen)
    {
        int openingDoor = 1; // one way
        if (!isOpen)
            openingDoor = -1; // another way

        return openingDoor;
    }
}
