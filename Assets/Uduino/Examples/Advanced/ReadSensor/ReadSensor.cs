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
    bool firstMeasure=true;
    void Awake()
    {
        
        UduinoManager.Instance.OnDataReceived += OnDataReceived; //Create the Delegate

    }
    
    void Update()
    {
        UduinoDevice myDevice = UduinoManager.Instance.GetBoard("Arduino");
        UduinoManager.Instance.Read(myDevice, "mySensor"); // Read every frame the value of the "mySensor" function on our board. 
    }
    int i;

    float value = 0;
    float oldvalue = 0;
    float firsValue = 0;
    public void OnDataReceived(string data, UduinoDevice device)
    {
        if (firstMeasure)
        {
            
            value= float.Parse(data);
       
            firstMeasure = false;
        }
        if (i%5 == 0) {

             value = float.Parse(data);

        }
        float dx= value- oldvalue;
       // Debug.Log("dx " + dx);

        oldvalue = value;

        //Debug.Log(data); // Use the data as you want !
        
        float x= MathF.Abs((float.Parse(data) - 910))/10;
        Debug.Log(value);
        //  _light.intensity = float.Parse(data) / 200;
        _light.intensity = x;

        cube.GetComponent<Renderer>().material.color = new Color(x,0,0);
        //leafs.color= new Color(x,.9f, .4f);
    }
}