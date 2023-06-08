using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class transferOwnership : MonoBehaviourPun
{
    private void OnMouseDown()
    {
    }
    private void OnEnable()
    {
        if (this.gameObject.transform.name == "player2")
        {
            base.photonView.RequestOwnership();
        }
    }

}
