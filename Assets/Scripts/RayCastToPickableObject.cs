using Assets.Player.Scripts;
using Assets.Scripts.Interfaces;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace Assets.Scripts
{
    public class RayCastToPickableObject : MonoBehaviour
    {
        [SerializeField] private Transform handsTransform;
        [SerializeField] private float rayLength;

        private RayCastingObjects rayCastingObjects;
        private GameObject objectInHand;

        private void Awake()
        {
            rayCastingObjects = GetComponent<RayCastingObjects>();
        }

        private void Update()
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (rayCastingObjects.ShotRay(rayLength))//, out objectInHand))
                {
                    ActionWithPickUpObject(rayCastingObjects.ShotRay(rayLength));
                }

            }

            if (objectInHand != null)
            {
                MoveObject();
            }
        }

        private void ActionWithPickUpObject(GameObject hitObject)
        {
            if (objectInHand == null)
            {
                if (hitObject.GetComponent(typeof(IPickupable)))
                {
                    objectInHand = hitObject;
                    objectInHand.GetComponent<IPickupable>().PickUp(handsTransform);
                }
            }
            else
            {
                DropObject();
            }

        }

        void DropObject()
        {
            var rb = objectInHand.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.drag = 1f;
            rb.transform.parent = null;
            objectInHand = null;
        }

        private void MoveObject()
        {
            objectInHand.transform.position = handsTransform.position;
        }
    }


}