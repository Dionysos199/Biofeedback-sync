using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class movement : MonoBehaviour
{
    public float walkSpeed=4;
    public float maxVelocityChange = 10;
    // Start is called before the first frame update
    private Vector2 input;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        input.Normalize();

        PhotonView pv = GameObject.Find("player3").GetComponent<PhotonView>();
        pv.RPC("ReceiveFloat", RpcTarget.All, transform.position.x);

    }
    private void FixedUpdate()
    {
        rb.AddForce(calculateMovement(walkSpeed), ForceMode.VelocityChange);
    }
    Vector3 calculateMovement(float _speed)
    {
        Vector3 targetVelocity = new Vector3(input.x, 0, input.y);
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= _speed;

        Vector3 velocity = rb.velocity;
        if (input.magnitude>.5f)
        {
            Vector3 velocityChange = targetVelocity - velocity;
            velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
            velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
            velocityChange.y = 0;
            return velocityChange;
        }
       else

        {
            return new Vector3();
        }
    }
}
