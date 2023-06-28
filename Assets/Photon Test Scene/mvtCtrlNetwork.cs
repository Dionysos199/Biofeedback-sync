using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Uduino;
using Photon.Pun;
using static UnityEngine.Rendering.DebugUI;
using Unity.Mathematics;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;

#endif

[System.Serializable]
public class mvtCtrlNetwork : MonoBehaviour
{
    //public Transform rotator;

    private PhotonView photonView;

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

    // Settings for input devices etc.
    [Header("Developer Settings")]
    public ControlDevice controlDevice;
    public enum ControlDevice { Controller, Keys, PhysicalSensor }
    [HideInInspector] public float keySteps = 0.1f;
    [HideInInspector] public Vector2 sensorValues;

    private InputDevice leftController;
    private InputDevice rightController;

    private float leftTilt = 0.5f;
    private float rightTilt = 0.5f;
    private float phaseShift = 0;

    private bool MaxReached1;
    private bool MaxReached2;


    // Start is called before the first frame update
    private void Awake()
    {

    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
  
        //ReadInput();
        Move();
    }
    int i;
    [PunRPC]
    void ReceiveFloat(float rotation, bool MaxReached,int playerIndex)
    {
        if (playerIndex == 1)
        {
            if (MaxReached)
            {
                i++;
            }
            Debug.Log("leftRotation  " + rotation+" max reached"+MaxReached+ i);
            leftTilt = rotation;
            MaxReached1 = MaxReached ;
        }
        if (playerIndex == 2)
        {

            Debug.Log("rightRotation  " + rotation);
            rightTilt = rotation;

            MaxReached2 = MaxReached;
        }
        Debug.Log(playerIndex);
    }



    float lastMeasure;

    void Move()
    {
        if (MaxReached1 )
        {

            float dt = Time.time - lastMeasure;
            lastMeasure = Time.time;

            Debug.Log("breath again" + dt);
        }

        if (MaxReached2)
        {
        }


        float pitch = 0;
        float yaw = 0;
        float roll=0;
        // Set rotation
        switch (navigationMode)
        {
            case NavigationMode.Differential:
                pitch = 1 - leftTilt - rightTilt;
                yaw = rightTilt - leftTilt;

                transform.Rotate(new Vector3(pitch, yaw, 0) * rotationSpeed * Time.deltaTime);
                break;
            case NavigationMode.PhaseShift:
              //  pitch = Mathf.Sin(phaseShift * Mathf.Deg2Rad);
                //yaw = Mathf.Cos((phaseShift - 90) * Mathf.Deg2Rad);
                roll = math.abs( leftTilt - rightTilt);
                Debug.Log("right"+rightTilt + "left" + leftTilt + "roll  " + roll );

                transform.Rotate(new Vector3(0, 0, roll) * rotationSpeed * Time.deltaTime);
                break;
            default:
                break;
        }

        // Tilt flippers
       // leftFlipper.transform.localRotation = Quaternion.AngleAxis((0.5f * (pitch + yaw)) * tiltRange, Vector3.right);
        //rightFlipper.transform.localRotation = Quaternion.AngleAxis((0.5f * (pitch - yaw)) * tiltRange, Vector3.right);

        // Rotate

        // Move forward
        transform.Translate(-Vector3.up * thrust * Time.deltaTime);
    }
}

