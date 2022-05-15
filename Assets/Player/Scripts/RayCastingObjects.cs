using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastingObjects : MonoBehaviour
{
    [SerializeField] float rayLength;

    private PlayerInput input;
    private Transform cameraTransform;
    private Ray ray;
    private RaycastHit hit;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();

        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {

        if (input.leftMouseButtonClick)
        {
            ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out hit, rayLength))
            {
                if (hit.collider.GetComponent(typeof(IInteractable)))
                {
                   hit.collider.GetComponent<IInteractable>().InteractWithObject();
                }
            }

            input.leftMouseButtonClick = false;
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (cameraTransform == null) return;
        Gizmos.DrawRay(cameraTransform.transform.position, cameraTransform.forward * rayLength);
    }
}
