using Photon.Pun;
using StableDiffusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class speechToText : MonoBehaviour
{
    string text1;
    string text2;
    public UnityEvent stableDiffusion;
    public Txt2Img _txt2Img;

   

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
        string mergedText=text1+text2;
        Debug.Log("finally merged thought" + text1+ text2);

        _txt2Img.txt2imgInput = new Txt2ImgPayload(mergedText);

        stableDiffusion?.Invoke();
    }
}