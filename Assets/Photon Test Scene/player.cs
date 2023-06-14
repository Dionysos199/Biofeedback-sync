using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

using Photon.Realtime;
using UnityEngine.UIElements;
using Uduino;

public class player : MonoBehaviour
{
    float rotationStep=.1f;
    float rotation;
    PhotonView pv;
    PhotonView MyPV;
    int ActorNm;

    public float inputMin = 950;
    public float inputMax = 980;

    public float outputMin = 0;
    public float outputMax = 5;


    private float scale;
    Smoother smoother = new Smoother(bufferSize: 20);

    // Start is called before the first frame update
    private void Awake()
    {
       UduinoManager.Instance.OnDataReceived += readSensor; //Create the Delegate
    }
    void Start()
    {

        pv = GameObject.Find("Bone").GetComponent<PhotonView>();
        MyPV = GetComponent<PhotonView>();
       ActorNm  = MyPV.OwnerActorNr;
    }
    void readSensor(string data, UduinoDevice device)
    {
        float inputValue = float.Parse(data);
        // Perform the mapping
        float normalizedValue = (inputValue - inputMin) / (inputMax - inputMin);
        float mappedValue = normalizedValue * (outputMax - outputMin) + outputMin;

        float smoothedValue = smoother.SmoothValue(mappedValue);
     
        Debug.Log("mapped value " + mappedValue);
        rotation = smoothedValue;
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
    void sendData()
    {
        if (pv)
        {
            pv.RPC("ReceiveFloat", RpcTarget.All, rotation, ActorNm);

        }
        else
        {
            Debug.Log("photon view object was not found hahaha");
        }
    }
}
