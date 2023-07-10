using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Narrator1 : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(Intro());

    }

    IEnumerator Intro()
    {
        yield return new WaitForSeconds(5f);
        SpeechManager.StartReadMessage("Hello and welcome to our experience! I will guide you through this little adventure to your innerself");
    }
}
