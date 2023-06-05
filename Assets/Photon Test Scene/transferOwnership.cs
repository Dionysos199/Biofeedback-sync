using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class transferOwnership : MonoBehaviourPun
{
    private void OnMouseDown()
    {
        base.photonView.RequestOwnership();
    }

}
