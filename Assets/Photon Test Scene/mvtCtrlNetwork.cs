using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Uduino;
using Photon.Pun;
using static UnityEngine.Rendering.DebugUI;
using Unity.Mathematics;
using Unity.VisualScripting;
using System;
using ExitGames.Client.Photon;
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

    public float leftTilt = 0.5f;
    public float rightTilt = 0.5f;
    private float phaseShift = 0;

    private bool MaxReached1;
    private bool MaxReached2;


    private InputDevice headsetDevice;
    private Quaternion lastRotation;


    // Start is called before the first frame update
    private void Awake()
    {

    }
    void Start()
    {
        lastRotation = Quaternion.identity;

    }
  

  
        // Invoke the event and pass the eventData to all registered listeners
      
    

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
            Debug.Log("leftRotation  " + rotation + " max reached" + MaxReached + i);
 
            leftTilt = rotation;
            MaxReached1 = MaxReached;
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
    float dt;
    float last_dt;
    public float lerpDt;
    void CalculatePhaseSHift()
    {
        if (MaxReached1)
        {

            last_dt = dt;
            dt = Time.time - lastMeasure;
        }

        if (MaxReached2)
        {
            lastMeasure = Time.time;
        }

        lerpDt = Mathf.Lerp(last_dt, dt, (Time.time - lastMeasure));
    }
    Vector3 headRotation ()
    {
        var head = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.Head, head);


        // Try to get the rotation feature from the headset device
        head[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out Quaternion headRotation);
        return (headRotation.eulerAngles);
    }
    void Move()
    {
   
        //CameraController.r_sphere.x = Mathf.Lerp(CameraController.r_sphere.x,dt,5);
        CalculatePhaseSHift();
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
                yaw = math.abs(leftTilt - rightTilt);

                Debug.Log("right"+rightTilt + "left" + leftTilt + "roll  " + roll );

                Debug.Log("breath again" + lerpDt);

                transform.Rotate(new Vector3(0,0,yaw*dt) * rotationSpeed * Time.deltaTime);
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

