using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RayCastingObjects : MonoBehaviour
{
    [SerializeField] float rayLength;

    private Transform cameraTransform;
    private Ray ray;
    private RaycastHit hit;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out hit, rayLength))
            {
                if (hit.collider.GetComponent(typeof(IInteractable)))
                {
                   hit.collider.GetComponent<IInteractable>().InteractWithObject();
                }
            }
        }       
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (cameraTransform == null) return;
        Gizmos.DrawRay(cameraTransform.transform.position, cameraTransform.forward * rayLength);
    }
}
