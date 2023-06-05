using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class calcPos : MonoBehaviour
{
    
    float pos;
    float averagePos;


    [PunRPC]
    public void ReceiveFloat(float ABpos)
    {
        averagePos= (pos + ABpos) / 2;
        pos = ABpos;

        Debug.Log("Average position " + averagePos);

    }
    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(averagePos, 1, 0);
    }
}
