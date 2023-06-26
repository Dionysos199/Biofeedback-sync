using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


public class UIFollowHead : MonoBehaviour
{
    [SerializeField] private Transform headTransform; // Reference to the VR head transform
    [SerializeField] private Vector3 offset; // Offset values for the UI element

    private void Update()
    {
        if (headTransform != null)
        {
            // Calculate the final position by adding the offset to the head position
            Vector3 targetPosition = headTransform.position + headTransform.forward * offset.z
                + headTransform.right * offset.x + headTransform.up * offset.y;

            // Set the position and rotation of the UI element to match the calculated position and the head rotation
            transform.position = targetPosition;
            transform.rotation = headTransform.rotation;
        }
        else
        {
            // If the head transform is not assigned, try to find it in the scene
            headTransform = Camera.main.transform;
        }
    }
}

