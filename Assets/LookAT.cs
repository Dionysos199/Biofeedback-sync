using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAT : MonoBehaviour
{
    public Transform target;

    void Update()
    {

       //transform.LookAt(transform.position+ Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        transform.LookAt(transform.position);


        //// Rotate the camera every frame so it keeps looking at the target
        //transform.LookAt(target);

        //// Same as above, but setting the worldUp parameter to Vector3.left in this example turns the camera on its side
        //transform.LookAt(target, Vector3.left);
    }
}
