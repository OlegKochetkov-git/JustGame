using Assets.Player.Scripts;
using Assets.Scripts.Interfaces;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts
{
    public class RayCastToInteractableObject : MonoBehaviour
    {
        [SerializeField] float rayLength;

        private RayCastingObjects rayCastingObjects;

        private void Awake()
        {
            rayCastingObjects = GetComponent<RayCastingObjects>();
        }

        private void Update()
        {
            InputHandleForLaunchRay();
        }

        private void InputHandleForLaunchRay()
        {
            if (!Keyboard.current.eKey.wasPressedThisFrame) return;

            GameObject hitableObject = rayCastingObjects.ShotRay(rayLength);
            if (hitableObject == null) return;

            ActionWithHittableObject(hitableObject);
        }

        private void ActionWithHittableObject(GameObject hitObject)
        {
            if (!hitObject.GetComponent(typeof(IInteractable))) return;

            hitObject.GetComponent<IInteractable>().InteractWithObject();
        }
    }
}