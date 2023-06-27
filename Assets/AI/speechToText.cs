using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class speechToText : MonoBehaviour
{
    string text1;
    string text2;
    [PunRPC]
    void ReceiveString(string text, int playerIndex)
    {
        if (playerIndex == 1)
        {
            Debug.Log("the text was received halleloua"+ text);
            text1 = text;
        }
        if (playerIndex == 2)
        {
            Debug.Log("the text was received halleloua" + text);
            text2 = text;
        }

        Debug.Log("finally merged thought" + text1+ text2);
    }
}