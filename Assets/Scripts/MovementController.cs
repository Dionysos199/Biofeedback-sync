using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Uduino;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

[System.Serializable]
public class MovementController : MonoBehaviour
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
    public float thrust = 0.1f;
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

    public float inputMin = 820;
    public float inputMax = 980;
    float outputMin = 0;
    float outputMax = 5;

    // Start is called before the first frame update
    private void Awake()
    {

        UduinoManager.Instance.OnDataReceived += sensorCtrl; //Create the Delegate
    }
    void Start()
    {
        sensorValues = new Vector2(0.5f, 0.5f);

	    if(controlDevice == ControlDevice.Controller)
            Invoke("TryInitialize", 1);
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
    	Move();
    }

    void TryInitialize()
    {
        // Get left controller
        List<InputDevice> left = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Left, left);
        if(left.Count > 0)
            leftController = left[0];

        // Get right controller
        List<InputDevice> right = new List<InputDevice>();
        InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Right, right);
        if(right.Count > 0)
            rightController = right[0];
    }

    void sensorCtrl(string data, UduinoDevice device)
    {

        float inputValue = float.Parse(data);

        float normalizedValue = (inputValue - inputMin) / (inputMax - inputMin);
        sensorValues.x= normalizedValue;
        Debug.Log(normalizedValue + " normalized value");
    }
    void ReadInput()
    {
        var values = new Vector2(leftTilt, rightTilt);

        switch(controlDevice)
        {
            case ControlDevice.Controller:
                if(leftController != null)
                {
                    // Left controller
                    leftController.TryGetFeatureValue(CommonUsages.trigger, out values.x);
                    if(values.x == 0)
                        values.x += 0.1f;
                }
     	        else
  	                Debug.LogError("Left controller is missing");

                if(rightController != null)
                {
                    // Right controller
                    rightController.TryGetFeatureValue(CommonUsages.trigger, out values.y);
                    if(values.y == 0)
                        values.y += 0.1f;
                }
                else
                    Debug.LogError("Right controller is missing");

                break;
            case ControlDevice.Keys:
                if(Input.GetKeyDown(KeyCode.W) && values.x < 1)
                    values.x += keySteps;
                if(Input.GetKeyDown(KeyCode.S) && values.x > 0)
                    values.x -= keySteps;
                if(Input.GetKeyDown(KeyCode.UpArrow) && values.y < 1)
                    values.y += keySteps;
                if(Input.GetKeyDown(KeyCode.DownArrow) && values.y > 0)
                    values.y -= keySteps;

                break;
            case ControlDevice.PhysicalSensor:
                values = sensorValues;

                break;
            default:
                break;
        }

        switch(navigationMode)
        {
            case NavigationMode.Differential:
                leftTilt = values.x;
                rightTilt = values.y;

                break;
            case NavigationMode.PhaseShift:
                // For now reading sensor values as f(x) = sin(x - a) + 1
                // to calculate phase shift
                phaseShift = 360 * (Mathf.Asin(1 - values.x) - Mathf.Asin(1 - values.y)) / (2 * Mathf.PI);
                if(phaseShift < -180 || phaseShift > 180)
                    Debug.LogError("phaseShift out of range");
                break;
            default:
                break;
        }
    }

    void Move()
    {
        float pitch = 0;
        float yaw = 0;

        // Set rotation
        switch(navigationMode)
        {
            case NavigationMode.Differential:
                pitch = 1 - leftTilt - rightTilt;
                yaw = rightTilt - leftTilt;

                break;
            case NavigationMode.PhaseShift:
                pitch = Mathf.Sin(phaseShift * Mathf.Deg2Rad);
                yaw = Mathf.Cos((phaseShift - 90) * Mathf.Deg2Rad);

                break;
            default:
                break;
        }

        // Tilt flippers
        leftFlipper.transform.localRotation = Quaternion.AngleAxis((0.5f * (pitch + yaw)) * tiltRange, Vector3.right);
        rightFlipper.transform.localRotation = Quaternion.AngleAxis((0.5f * (pitch - yaw)) * tiltRange, Vector3.right);

        // Rotate
        transform.Rotate(new Vector3(pitch, yaw, 0) * rotationSpeed * Time.deltaTime);

        // Move forward
        transform.Translate(Vector3.forward * thrust * Time.deltaTime);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MovementController))]
public class MovementControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the target as the used type
        var movementController = target as MovementController;

        // Set custom styles
        GUIStyle wrappedLabel = new GUIStyle(GUI.skin.GetStyle("label")) { wordWrap = true };

        // Create Editor GUI
        base.OnInspectorGUI();
        switch(movementController.controlDevice)
        {
            case MovementController.ControlDevice.Keys:
                movementController.keySteps = EditorGUILayout.FloatField("Key Steps", movementController.keySteps);
                break;
            case MovementController.ControlDevice.PhysicalSensor:
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Map sensor values to range from 0 to 1 and apply them to variable `sensorValues` of type Vector2.", wrappedLabel);
                EditorGUILayout.EndVertical();
                movementController.sensorValues = EditorGUILayout.Vector2Field("Sensor Values", movementController.sensorValues);
                break;
            default:
                break;
        }

        // Apply changes to the serializedProperty - always do this at the end of OnInspectorGUI.
        serializedObject.ApplyModifiedProperties();
    }
}
#endif