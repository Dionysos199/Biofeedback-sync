using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using static UnityEngine.Rendering.DebugUI;

public class testRotationPhoton : MonoBehaviour
{
    public InputDevice leftController;
    public XRController rightController;

    private InputDevice targetDevice;
    float value;

    public InputDeviceCharacteristics controllerCharacteristics;
    // Start is called before the first frame update
    void Start()
    {
        TryInitialize();
    }
    void TryInitialize()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);
        if (devices.Count > 0)
        {
            targetDevice = devices[0];

        }
    }
        // Update is called once per frame
        void Update()
    {

        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
        {
            Debug.Log("hihi" + triggerValue);
   
        }
    }
    void simpleRotate()
    {
        transform.rotation= Quaternion.Euler(0, Time.time, 0);
    }
}
