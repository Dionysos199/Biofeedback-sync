using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class takeOver : MonoBehaviour
{
    PhotonView _photonView;
    // Start is called before the first frame update
    void Start()
    {
        _photonView = GetComponent<PhotonView>();

    }

    // Update is called once per frame
    void Update()
    {
    }
}
