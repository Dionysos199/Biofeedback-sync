using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed = 5f;

    private void Update()
    {
        // Move the object forward based on its local forward direction
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}

