using System.Collections;
using System.Collections.Generic;
using Uduino;
using UnityEngine;
using UnityEngine.UIElements;

public class levitate : MonoBehaviour
{
    public float inputMin = 820;
    public float inputMax = 980;

    float outputMin = 0;
    public float outputMax = 20;

    float smoothenedValue;
    public Transform avatar;
    
    // Start is called before the first frame update
    void Awake()
    {
        UduinoManager.Instance.OnDataReceived += updateHeight; // Create the Delegate
    }

    SignalProcessor processor = new SignalProcessor(bufferSize: 20);
    public void updateHeight(string data, UduinoDevice device)
    {
        float reading = int.Parse(data);
        processor.AddValue(reading);

        // Process reading
        float value = processor.GetNormalized();

        avatar.transform.position = new Vector3(0, value, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
