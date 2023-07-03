
using Photon.Pun;
using UnityEngine;

using Uduino;
using Whisper;

public class player : MonoBehaviour
{
    float rotationStep=.1f;
    float rotation;
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

    private void Update()
    {
        UduinoDevice board = UduinoManager.Instance.GetBoard("Arduino");
        UduinoManager.Instance.Read(board, "readSensors"); // Read every frame the value of the "readSensors" function on our board.
       
        if (singleton.text!="")
        {
            if (MyPV.IsMine)
            {
                sendText();

            }

        }
    }
    void readSensor(string data, UduinoDevice device)
    {
        float inputValue = float.Parse(data);

        processor.AddValue(inputValue);
        rotation = processor.GetNormalized();
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
            pv.RPC("ReceiveFloat", RpcTarget.All, rotation,processor.MaxReached(), ActorNm);

        }
        else
        {
            Debug.Log("photon view object was not found hahaha");
        }
    }
}
