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

        UduinoManager.Instance.OnDataReceived += updateHeight; //Create the Delegate

    }

    Smoother smoother = new Smoother(bufferSize: 20);
    public void updateHeight(string data, UduinoDevice device)
    {
        float inputValue = float.Parse(data);
        // Perform the mapping
        float normalizedValue = (inputValue - inputMin) / (inputMax - inputMin);
        float mappedValue = normalizedValue * (outputMax - outputMin) + outputMin;

        smoothenedValue = smoother.SmoothValue(mappedValue);
        avatar.transform.position = new Vector3(0, smoothenedValue, 0);

    }
    // Update is called once per frame
    void Update()
    {

    }
}
