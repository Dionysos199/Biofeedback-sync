using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Narrator : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
        SpeechManager.StartReadMessage("Hello and welcome to our experience! I will guide you threw this little adventure to your innerself ");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
