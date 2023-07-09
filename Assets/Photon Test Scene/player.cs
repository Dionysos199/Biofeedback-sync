
using Photon.Pun;
using UnityEngine;

using Uduino;
using Whisper;
using UnityEngine.Events;
using Unity.VisualScripting;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;
using System;

public class player : MonoBehaviour
{
    Vector3 headRotation;
    float rotationStep=.1f;
    float sensorValue;
    PhotonView pv;
    PhotonView AIpv;
    PhotonView MyPV;
    int ActorNm;
    SignalProcessor processor;

    // Start is called before the first frame update
    private void Awake()
    {
       UduinoManager.Instance.OnDataReceived += readSensor; //Create the Delegate

    }
    void Start()
    {
        processor = new SignalProcessor(20, true);
        pv = GameObject.Find("Bone").GetComponent<PhotonView>();
        AIpv = GameObject.Find("combineTexts").GetComponent<PhotonView>();

        MyPV = GetComponent<PhotonView>();
        ActorNm  = MyPV.OwnerActorNr;

        // Add invoke for resetting auto range
    }

    // Add auto reset function
    string lastText="";
    private void Update()
    {
        UduinoDevice board = UduinoManager.Instance.GetBoard("Arduino");
        UduinoManager.Instance.Read(board, "readSensors"); // Read every frame the value of the "readSensors" function on our board.
        headRotation = HeadRotation();
        string text= singleton.text;
        if (text != lastText)
        {
            if (MyPV.IsMine)
            {
                sendText();

            }
        }
        lastText = text;

        
    }
    Vector3 HeadRotation()
    {
        var head = new List<UnityEngine.XR.InputDevice>();
        UnityEngine.XR.InputDevices.GetDevicesAtXRNode(UnityEngine.XR.XRNode.Head, head);
        Quaternion rotation = new Quaternion();
        if (head.Count > 0)
        {
            head[0].TryGetFeatureValue(UnityEngine.XR.CommonUsages.deviceRotation, out Quaternion headRotation);
            rotation = headRotation;
        }
        return (rotation.eulerAngles);
    }
    void readSensor(string data, UduinoDevice device)
    {

        float inputValue;
        bool success = float.TryParse(data, out inputValue );
        if (success)
        {
            Console.WriteLine($"Converted '{data}' to {inputValue}.");
        }
        else
        {
            Console.WriteLine($"Attempted conversion of '{data ?? "<null>"}' failed.");
        }

        processor.AddValue(inputValue);
        sensorValue = processor.GetNormalized();
        processor.extremum();
   

        if (MyPV.IsMine)
        {
            sendData();

        }
    }
    // Update is called once per frame
    /*   private void Update()
       {
           if (Input.GetKeyDown("s"))
           {
               rotation -= .1f;
               Debug.Log("rotation");

           }
           if (Input.GetKeyDown("w"))
           {
               rotation += .1f;

           }
           if (MyPV.IsMine)
           {
               sendData();

           }
       }
   */
    void sendText()
    {
        if (AIpv)
        {
            AIpv.RPC("ReceiveString", RpcTarget.All, singleton.text, ActorNm);

        }
        else
        {
            Debug.Log("photon view object was not found hahaha");
        }
    }
    void sendData()
    {
        if (pv)
        {
            pv.RPC("ReceiveFloat", RpcTarget.All, sensorValue,processor.MaxReached(),headRotation, ActorNm);

        }
        else
        {
            Debug.Log("photon view object was not found hahaha");
        }
    }
}
