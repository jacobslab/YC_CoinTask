using System.Collections;
using System.Collections.Generic;
using UnityEngine.XR;
using UnityEngine;


public class OculusGoControllerTester : MonoBehaviour
{

    public Transform controller;

    public static bool leftHanded { get; private set; }


    InputDevice targetDevice;
    OVRInput.Controller activeController;
    // Start is called before the first frame update
    void Start()
    {
        /*
        var gameControllers = new List<InputDevice>();
        InputDevices.GetDevicesWithRole(InputDeviceRole.GameController, gameControllers);

        foreach (var device in gameControllers)
        {
            targetDevice = device;
            UnityEngine.Debug.Log(string.Format("Device Name '{0}' has role '{1}'", device.name, device.role.ToString()));
        }
        */
        activeController = OVRInput.GetActiveController();


        if (OVRInput.IsControllerConnected(OVRInput.Controller.RTrackedRemote))
        {
            Debug.Log("NOT RIGHT REMOTE");
        }


        if (OVRInput.IsControllerConnected(OVRInput.Controller.LTrackedRemote))
        {
            Debug.Log("NOT LEFT REMOTE");
        }

                
    }

    void Awake()
    {
#if UNITY_EDITOR
        leftHanded = false;        // (whichever you want to test here)
#else
        leftHanded = OVRInput.GetControllerPositionTracked(OVRInput.Controller.LTouch);
#endif
    }

    // Update is called once per frame
    void Update()
    {

       // Debug.Log("velocity " + OVRInput.GetLocalControllerVelocity(OVRInput.Controller.RTouch).ToString());
        //Debug.Log("accceleration " + OVRInput.GetLocalControllerAcceleration(OVRInput.Controller.RTouch).ToString());
        //Debug.Log("position " + OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch).ToString());
        //Debug.Log("ACTIVE CONTROLLER: " + OVRInput.GetActiveController().ToString());

        //Debug.Log("TOUCHPAD HORIZONTAL " + Input.GetAxis("TouchpadHorizontal").ToString());
        //  Debug.Log("TOUCHPAD VERTICAL " + Input.GetAxis("TouchpadVertical").ToString());

        /*
if(Input.GetButton("TouchpadButton"))
{
    UnityEngine.Debug.Log("PRESSED TOUCHPAD BUTTON");
}

if(Input.GetButton("OculusTrigger"))
{
    UnityEngine.Debug.Log("PRESSED OCULUS TRIGGER");
}

bool triggerButton;
if(targetDevice.TryGetFeatureValue(CommonUsages.triggerButton, out triggerButton))
{
    UnityEngine.Debug.Log("XR pressed TRIGGER BUTTON");
}

OVRInput.Controller c = leftHanded ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;
if (OVRInput.GetControllerPositionTracked(c))
{
    controller.localRotation = OVRInput.GetLocalControllerRotation(c);
    controller.localPosition = OVRInput.GetLocalControllerPosition(c);
}

UnityEngine.Debug.Log("POSITION " + controller.localPosition.ToString());
UnityEngine.Debug.Log("ROTATION " + controller.localRotation.ToString());
*/
    }
}
