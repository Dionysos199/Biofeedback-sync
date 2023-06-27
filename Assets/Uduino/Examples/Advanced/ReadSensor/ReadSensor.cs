using UnityEngine;
using System.Collections;
using Uduino;

public class ReadSensor : MonoBehaviour
{

    UduinoManager u;

    void Awake()
    {
        UduinoManager.Instance.OnDataReceived += OnDataReceived; //Create the Delegate
    }

    void Update()
    {
        UduinoDevice myDevice = UduinoManager.Instance.GetBoard("Arduino");
        UduinoManager.Instance.Read(myDevice, "sensor1"); // Read every frame the value of the "mySensor" function on our board.
        UduinoManager.Instance.Read(myDevice, "sensor2");
    }

    public void OnDataReceived(string data, UduinoDevice device)
    {
        Debug.Log(data); // Use the data as you want !
    }

    void Read()
    {

    }
}