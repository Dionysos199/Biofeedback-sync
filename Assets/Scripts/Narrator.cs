using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Narrator : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        SpeechManager.StartReadMessage("Hello my name is Bond, James Bond ");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
