using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UIElements;
using Uduino;

public class player : MonoBehaviour
{
    public SignalProcessing signalProcessing;
    public enum SignalProcessing { Amplitude, Frequency, PhaseShift }
    private PhotonView bodyPV;
    private PhotonView playerPV;
    private int actorNum;

    private UduinoManager uduino;

    private SignalProcessor processor;

    void Awake()
    {
        // Create the Delegate
        UduinoManager.Instance.OnDataReceived += OnDataReceived;

        // Photon networking setup
        bodyPV = GameObject.Find("Body").GetComponent<PhotonView>();
        playerPV = GetComponent<PhotonView>();
        actorNum  = playerPV.OwnerActorNr;

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

        // Calculate frequency
        var frequency = processor.GetFrequency();
        Debug.Log("frequency: " + frequency);

        // Calculate phase shift coefficient
        var coeff = processor.GetPhaseShiftCoeff();
        Debug.Log("coeff: " + coeff);

        if (playerPV.IsMine)
        {
            switch (signalProcessing)
            {
                case SignalProcessing.Frequency:
                    sendFloat(frequency, playerPV);
                    break;
                case SignalProcessing.PhaseShift:
                    if (!bodyPV.IsMine)
                        sendFloat(coeff, playerPV);
                    break;
                default:
                    // Amplitude
                    sendFloat(amplitude, bodyPV);
                    break;
            }
        }
    }

    [PunRPC]
    void ReceiveFloat(float coeff, int actorNum)
    {
        // Calculate phase shift if I have ownership of the Body
        if (bodyPV.IsMine)
        {
            var phaseShift = processor.GetPhaseShift(coeff);
            Debug.Log("remoteCoeff: " + coeff + ", phaseShift: " + phaseShift);
            sendFloat(phaseShift, bodyPV);
        }
    }

    void sendFloat(float value, PhotonView target)
    {
        target.RPC("ReceiveFloat", RpcTarget.All, value, actorNum);
    }
}
