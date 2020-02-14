using UnityEngine;
using UnityEngine.UI;

public class CustomLaserPointer : MonoBehaviour
{
    public LineRenderer laserLineRenderer1;
    public float laserWidth = 0.1f;
    public float laserMaxLength = 5f;
    public OvrAvatar ovrAvatar;

    //OVRCameraRig cameraRig;
    public Text text;

    void Start()
    {
        Vector3[] initLaserPositions = new Vector3[2] { Vector3.zero, Vector3.zero };
        laserLineRenderer1.SetPositions(initLaserPositions);
        laserLineRenderer1.SetWidth(laserWidth, laserWidth);
        //cameraRig = FindObjectOfType<OVRCameraRig>();
        // ovrAvatar = FindObjectOfType<OvrAvatar>();
    }
    void Update()
    {
        //Debug.Log("oculus avatar " + ovrAvatar);
        //Debug.Log("oculus avatar hands  " + ovrAvatar.GetHandTransform(OvrAvatar.HandType.Left, OvrAvatar.HandJoint.HandBase).ToString());
        //if (OVRInput.Get(OVRInput.RawTouch.RIndexTrigger))
        // {
        ShootLaserFromTargetPosition(laserLineRenderer1,transform.position,transform.forward, laserMaxLength);
        // ShootLaserFromTargetPosition(laserLineRenderer2, transform.position, transform.right, laserMaxLength);
        //ShootLaserFromTargetPosition(laserLineRenderer3,transform.position, -transform.right, laserMaxLength);

        // ShootLaserFromTargetPosition(transform.position, transform.right, laserMaxLength);
        /*
        text.text = "laser pointer triggered"; //debug text
        laserLineRenderer.enabled = true;
    }
    else
    {
        text.text = "";
        laserLineRenderer.enabled = false;
    }
    */
    }
    void ShootLaserFromTargetPosition(LineRenderer chosenLine, Vector3 targetPosition, Vector3 direction, float length)
    {
        Ray ray = new Ray(targetPosition, direction);
        Vector3 endPosition = targetPosition + (length * direction);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, length))
        {
            endPosition = raycastHit.point;
        }
        chosenLine.SetPosition(0, targetPosition);
        chosenLine.SetPosition(1, endPosition);
    }
}