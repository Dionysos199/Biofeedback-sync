using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Narrator2 : MonoBehaviour
{

    void Start()
    {
        StartCoroutine(Intro());

    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(5f);
        SpeechManager.StartReadMessage("Welcome to Scene 2");
    }

}

