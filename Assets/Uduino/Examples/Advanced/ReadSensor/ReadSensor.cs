using UnityEngine;
using System.Collections;
using Uduino;
using System;


public class ReadSensor : MonoBehaviour
{
    public Light _light;
    public    GameObject cube;
    UduinoManager u;
    public Material leafs;

    void Awake()
    {
        UduinoManager.Instance.OnDataReceived += OnDataReceived; //Create the Delegate
    }

    void Update()
    {
        UduinoDevice myDevice = UduinoManager.Instance.GetBoard("ImadsUno");
        UduinoManager.Instance.Read(myDevice, "mySensor"); // Read every frame the value of the "mySensor" function on our board. 
    }

    public void OnDataReceived(string data, UduinoDevice device)
    {
        Debug.Log(data); // Use the data as you want !
        float x= (float.Parse(data) - 855) * 10;
        Debug.Log(x);
        //  _light.intensity = float.Parse(data) / 200;
        _light.intensity = x;
        cube.GetComponent<Renderer>().material.color = new Color(x,0,0);
        //leafs.color= new Color(x,.9f, .4f);
    }
}