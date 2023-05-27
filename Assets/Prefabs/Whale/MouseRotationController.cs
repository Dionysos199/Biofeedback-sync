using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

using UnityEngine.XR.Interaction.Toolkit;
public class MouseRotationController : MonoBehaviour
{
    public float sensitivity = 2f; // Mouse movement sensitivity

    private float rotationX; // Rotation around the X-axis
    private float rotationY; // Rotation around the Y-axis

    public float speed = 5f; // Movement speed

    private void Update()
    {
        // Get mouse movement
        // float mouseX = Input.GetAxis("Mouse X");
        // float mouseY = Input.GetAxis("Mouse Y");

        // Calculate rotation based on mouse movement
        //  rotationX -= mouseY * sensitivity;
        // rotationY += mouseX * sensitivity;

        // Clamp rotation angles to desired range
        // rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        // rotationY = Mathf.Clamp(rotationY, -90f, 90f);
        var heads = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.Head, heads);
        UnityEngine.XR.InputDevice head = new UnityEngine.XR.InputDevice();
        if (heads.Count == 1)
        {
            head = heads[0];
        }


        head.TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out Quaternion headRotation);
        Debug.Log(headRotation.x + " " + headRotation.y + " " + headRotation.z);    
        

        // Create a quaternion representing a 90-degree rotation around the X-axis
        Quaternion additionalRotation = Quaternion.Euler(-90f, 0f, 0f);

        // Use the headset rotation value
        transform.rotation = headRotation*additionalRotation;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            // Move the object forward
            transform.Translate(-Vector3.up * speed * Time.deltaTime);
        }

        // Apply rotation to transform
        //transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
    }
}