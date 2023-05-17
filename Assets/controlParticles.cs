using UnityEngine;
using System.Collections;
using Uduino;
using System;


[RequireComponent(typeof(ParticleSystem))]
public class controlParticles : MonoBehaviour
{
    float initialTemp;
    bool firstMeasure=true;
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

        if (firstMeasure)
        {
            initialTemp = float.Parse(data);
            Debug.Log("initial temp"+initialTemp);
            firstMeasure = false;
        }
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var em = ps.emission;
        em.enabled = true;

        float mappedData = (float.Parse(data) - initialTemp)*10;
        Debug.Log("rate Over time" + mappedData);
        em.rateOverTime = mappedData;
        ps.startSpeed = mappedData;

    }

    void Read()
    {

    }
}