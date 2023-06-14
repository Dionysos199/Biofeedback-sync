using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UIElements;
using Uduino;

public class player : MonoBehaviour
{
    public float inputMin = 950;
    public float inputMax = 980;

    public float outputMin = 0;
    public float outputMax = 5;

    private float rotationStep=.1f;
    private float rotation;
    private PhotonView pv;
    private PhotonView myPV;
    private int actorNum;

    private float scale;
    private SignalProcessor processor;

    void Awake()
    {
        // Create the Delegate
        UduinoManager.Instance.OnDataReceived += ReadSensor;
    }

    void Start()
    {
        // Is Start necessary if we already have Awake? Can we just merge them?

        // Photon networking setup
        pv = GameObject.Find("Body").GetComponent<PhotonView>();
        myPV = GetComponent<PhotonView>();
        actorNum  = myPV.OwnerActorNr;

        // Instantiate signal processor
        processor = new SignalProcessor(bufferSize: 20);
    }

    void ReadSensor(string data, UduinoDevice device)
    {
        int reading = int.Parse(data);

        // Process reading
        float value = processor.GetNormalized();
        Debug.Log("value: " + value);

        if (MyPV.IsMine)
            sendFloat();
    }

    void sendFloat(float value)
    {
        pv.RPC("ReceiveFloat", RpcTarget.All, value, ActorNm);
    }
}
