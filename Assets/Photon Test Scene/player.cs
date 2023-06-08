using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

using Photon.Realtime;
public class player : MonoBehaviour
{
    float rotationStep=.1f;
    float rotation;
    PhotonView pv;
    PhotonView MyPV;
    int ActorNm;
    // Start is called before the first frame update
    void Start()
    {

        pv = GameObject.Find("Body").GetComponent<PhotonView>();
        MyPV = GetComponent<PhotonView>();
       ActorNm  = MyPV.OwnerActorNr;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) && rotation < 1)
        {
            rotation += rotationStep;
            if(MyPV.IsMine)
            {
                sendData();

            }
        }
        if (Input.GetKeyDown(KeyCode.S) && rotation > 0)
        {
            rotation -= rotationStep;
            if (MyPV.IsMine)
            {
                sendData();

            }
        }
    }
    void sendData()
    {
        pv.RPC("ReceiveFloat", RpcTarget.All, rotation,ActorNm);
    }
}
