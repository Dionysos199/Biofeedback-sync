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
    private float lastMin = 0;
    private float lastMax = 0;

    void Awake()
    {
        // Create the Delegate
        UduinoManager.Instance.OnDataReceived += OnDataReceived;

        // Photon networking setup
        pv = GameObject.Find("Body").GetComponent<PhotonView>();
        myPV = GetComponent<PhotonView>();
        actorNum  = myPV.OwnerActorNr;

        // Instantiate signal processor
        processor = new SignalProcessor(bufferSize: 20);

        // Reset auto-range for sensors
        Invoke("ResetSensor", 1);
    }

    void ResetSensor()
    {
        // Reset auto-range for sensors
        processor.ResetAutoRange();
        Debug.Log("Sensor range reset.");
    }

    void Update()
    {
        UduinoDevice board = UduinoManager.Instance.GetBoard("Arduino");
        UduinoManager.Instance.Read(board, "readSensors"); // Read every frame the value of the "readSensors" function on our board.
    }

    void OnDataReceived(string data, UduinoDevice device)
    {
        int reading = int.Parse(data);
        processor.AddValue(reading);

        // Detect peaks
        SignalProcessor.Peak peak = processor.DetectPeak();
        if (peak == SignalProcessor.Peak.Minimum)
            lastMin = processor.GetSmoothed();
        if (peak == SignalProcessor.Peak.Maximum)
            lastMax = processor.GetSmoothed();

        // Calculate amplitude
        var (min, max) = processor.GetLimits();
        var maxAmplitude = max - min;
        var currentAmplitude = lastMax - lastMin;

        // Normalize
        var tilt = 1 - processor.Normalize(currentAmplitude, (0, maxAmplitude));
        Debug.Log("tilt: " + tilt);

        if (myPV.IsMine)
            sendFloat(tilt);
    }

    void sendFloat(float value)
    {
        pv.RPC("ReceiveFloat", RpcTarget.All, value, actorNum);
    }
}
