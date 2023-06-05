using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class networkManager : MonoBehaviourPunCallbacks

{
    // Start is called before the first frame update
    void Start()
    {
        connectToServer();
    }

    void connectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("try Connect To Server");

    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("connected to Server.");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);

    }
    public override void OnJoinedRoom()
    {
        Debug.Log("joined a room"); 
        base.OnJoinedRoom();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("a new player joined the room");
        base.OnPlayerEnteredRoom(newPlayer); 
    }
    float posA;

    [PunRPC]
    void ReceiveFloat(float pos)
    {
        float AveragePos = (pos + posA) / 2;

    
        Debug.Log(pos);
    }
}
