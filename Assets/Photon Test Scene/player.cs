using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UIElements;
using Uduino;

public class player : MonoBehaviour
{
    private PhotonView pv;
    private PhotonView myPV;
    private int actorNum;

    private UduinoManager uduino;

    private SignalProcessor processor;

    void Awake()
    {
        // Create the Delegate
        UduinoManager.Instance.OnDataReceived += OnDataReceived;

        // Photon networking setup
        pv = GameObject.Find("Body").GetComponent<PhotonView>();
        myPV = GetComponent<PhotonView>();
        actorNum  = myPV.OwnerActorNr;

        // Instantiate signal processor
        processor = new SignalProcessor(bufferSize: 10, invertReadings: true);

        // Reset auto-range for sensors
        Invoke("ResetSensor", 1);
    }

    void ResetSensor()
    {
        // Reset auto-range for sensors
        processor.RequestAutoRangeReset();
        Debug.Log("Sensor range reset.");
    }

    void Update()
    {
        UduinoDevice board = UduinoManager.Instance.GetBoard("Arduino");
        UduinoManager.Instance.Read(board, "readSensors"); // Read every frame the value of the "readSensors" function on our board.
    }

    void OnDataReceived(string data, UduinoDevice device)
    {
        // Read sensor value
        int reading = int.Parse(data);
        processor.AddValue(reading);

        // Calculate amplitude
        var amplitude = processor.GetAmplitude();
        Debug.Log("amplitude: " + amplitude);

        if (myPV.IsMine)
            sendFloat(amplitude);
    }

    void sendFloat(float value)
    {
        pv.RPC("ReceiveFloat", RpcTarget.All, value, actorNum);
    }
}
