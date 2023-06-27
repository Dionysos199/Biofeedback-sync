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
    public NavigationMode navigationMode;
    public enum NavigationMode { Differential, PhaseShift }
    public float thrust = 0.7f;
    public float rotationSpeed = 10f;

    private float leftTilt = 0.5f;
    private float rightTilt = 0.5f;

    // Phase shift
    private int peakCounter = 0;
    private int peakDistance = 0;
    private int maxPeakDistance = 1;

    private PhotonView photonView;

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    
    [PunRPC]
    void ReceiveValues(float tilt, bool isMax, int playerIndex)
    {
        if (playerIndex == 1)
        {
            if (isMax)
                peakCounter++;

            Debug.Log("leftTilt: " + tilt + ", isMax: " + isMax + ", peakCounter: " + peakCounter);
            leftTilt = tilt;
        }
        if (playerIndex == 2)
        {
            if (isMax)
            {
                peakDistance = peakCounter;
                if (peakDistance > maxPeakDistance)
                    peakDistance = maxPeakDistance;
                peakCounter = 0;
            }

            Debug.Log("rightTilt: " + tilt);
            rightTilt = tilt;
        }
    }

    void Move()
    {
        float pitch = 0;
        float yaw = 0;

        switch (navigationMode)
        {
            case NavigationMode.Differential:
                // Set rotation
                pitch = 1 - leftTilt - rightTilt;
                yaw = rightTilt - leftTilt;
                break;
            case NavigationMode.PhaseShift:
                var shift = peakDistance / maxPeakDistance;
                // check against a threashold
                if (shift <= 0.2f)
                {
                    // Add navigation based on head movement/looking direction of both players
                }
                else
                {
                    // Add random movement
                }
                break;
            default:
                break;
        }

        // Tilt flippers (only for visual feedback, no effect on navigation)
        leftFlipper.transform.localRotation = Quaternion.AngleAxis((0.5f * (pitch + yaw)) * tiltRange, Vector3.right);
        rightFlipper.transform.localRotation = Quaternion.AngleAxis((0.5f * (pitch - yaw)) * tiltRange, Vector3.right);

        // Rotate
        transform.Rotate(new Vector3(pitch, yaw, 0) * rotationSpeed * Time.deltaTime);

        // Move forward
        transform.Translate(Vector3.forward * thrust * Time.deltaTime);
    }
}
