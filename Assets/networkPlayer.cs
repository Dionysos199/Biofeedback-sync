using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class networkPlayer : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    // Start is called before the first frame update
    void Start()
    {
        mapPosition(head, XRNode.Head);
        mapPosition(leftHand, XRNode.LeftHand);
        mapPosition(rightHand, XRNode.RightHand);
    }
    void mapPosition(Transform target,XRNode node)
    {
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position);
        InputDevices.GetDeviceAtXRNode(node).TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);
        target.position = position;
        target.rotation = rotation;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
