using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using static UnityEngine.Rendering.DebugUI;

public class MovementControlNetwork : MonoBehaviour
{
    // Physical body settings, no impact on navigation
    [Header("Body")]
    public GameObject leftFlipper;
    public GameObject rightFlipper;
    public int tiltRange = 30;

    // Navigation control
    [Header("Navigation Settings")]
    public float thrust = 0.7f;
    public float rotationSpeed = 10f;

    private float leftTilt = 0.5f;
    private float rightTilt = 0.5f;

    private PhotonView photonView;

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    
    [PunRPC]
    void ReceiveFloat(float tilt, int playerIndex)
    {
        Debug.Log("playerIndex: " + playerIndex);

        if (playerIndex == 1)
        {
            Debug.Log("leftTilt: " + tilt);
            leftTilt = tilt;
        }
        if (playerIndex == 2)
        {
            Debug.Log("rightTilt: " + tilt);
            rightTilt = tilt;
        }
    }

    void Move()
    {
        // Set rotation
        float pitch = 1 - leftTilt - rightTilt;
        float yaw = rightTilt - leftTilt;

        // Tilt flippers (only for visual feedback, no effect on navigation)
        leftFlipper.transform.localRotation = Quaternion.AngleAxis((0.5f * (pitch + yaw)) * tiltRange, Vector3.right);
        rightFlipper.transform.localRotation = Quaternion.AngleAxis((0.5f * (pitch - yaw)) * tiltRange, Vector3.right);

        // Rotate
        transform.Rotate(new Vector3(pitch, yaw, 0) * rotationSpeed * Time.deltaTime);

        // Move forward
        transform.Translate(Vector3.forward * thrust * Time.deltaTime);
    }
}
