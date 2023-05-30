
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class networkPlayer : MonoBehaviour
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    private PhotonView photonView;

    private Transform headRig;
    private Transform rightRig;
    private Transform leftRig;

    public Animator leftHandAnimator;
    public Animator rightHandAnimator;
    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        XROrigin rig= FindAnyObjectByType< XROrigin>();

        headRig = rig.transform.Find("Camera Offset/Main Camera");
        leftRig = rig.transform.Find("Camera Offset/LeftHand Controller");
        rightRig = rig.transform.Find("Camera Offset/RightHand Controller");
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            //rightHand.gameObject.SetActive(false);
            //leftHand.gameObject.SetActive(false);
            //head.gameObject.SetActive(false);

            mapPosition(head, headRig);
            mapPosition(leftHand, leftRig);
            mapPosition(rightHand, rightRig);

            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.LeftHand), leftHandAnimator);
            UpdateHandAnimation(InputDevices.GetDeviceAtXRNode(XRNode.RightHand), rightHandAnimator);
        }
    }

    void mapPosition(Transform target, Transform rigTransform)
    {

        target.position = rigTransform.position;
        target.rotation = rigTransform.rotation;
    }
    void UpdateHandAnimation(InputDevice targetDevice , Animator handAnimator)
    {
        if (targetDevice.TryGetFeatureValue(CommonUsages.trigger,out float triggerValue))
        {
            handAnimator.SetFloat("Trigger", triggerValue);
        }
        else
        {
            handAnimator.SetFloat("Trigger", 0);
        }

        if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            handAnimator.SetFloat("Grip", gripValue);
        }
        else
        {
            handAnimator.SetFloat("Grip", 0);
        }

    }
}