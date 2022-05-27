using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMotion : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject obj;
    public GameObject cam;
    public GameObject block_obj;
    public GameObject realplayer;
    public GameObject MazeObj;
    public GameObject MazeObj2;
    static public bool touched = false;

    private Touch touch;
    private float speedModifier;
    public float Force;
    void Start()
    {
        speedModifier = 0.001f;
        realplayer = GameObject.FindGameObjectWithTag("Player");
        Force = 10f;
        //realplayer.transform.GetComponent<Rigidbody>().AddForce(new Vector3(0, -9.81f, 0), ForceMode.Force);
    }

    // Update is called once per frame
    void Update()
    {
        //var rayCastPoint = Camera.main.;
        if (TouchScreenInputWrapper.touches.Length > 0)
        {
            Ray rayCastPoint = Camera.main.ScreenPointToRay(TouchScreenInputWrapper.touches[0].position);
            UnityEngine.Debug.Log("rayCastPoint origin: " + rayCastPoint.origin);
            UnityEngine.Debug.Log("rayCastPoint direction: " + rayCastPoint.direction);
            UnityEngine.Debug.Log("Player position: " + realplayer.transform.position);
            Debug.DrawRay(rayCastPoint.origin, rayCastPoint.direction * 10, Color.yellow);
            //Debug.DrawRay(rayCastPoint.origin, rayCastPoint.direction * 10, Color.yellow);
            RaycastHit hit;
            //obj.transform.position = rayCastPoint.origin;
            Force = 100f;
            UnityEngine.Debug.Log("TouchPhase: " + TouchScreenInputWrapper.touches[0].phase);
            if (TouchScreenInputWrapper.touches[0].phase == TouchPhase.Began)
            {
                touched = true;
                if (Physics.Raycast(rayCastPoint, out hit))
                {
                    if (hit.transform.gameObject.tag != "Player")
                    {
                Vector3 offSet = new Vector3(0f,0f,0f);
                Vector3 eA = realplayer.transform.eulerAngles;
                float rad = (Mathf.PI / 180f) * (eA.y);
                offSet.x = Mathf.Sin(rad);
                offSet.z = Mathf.Cos(rad);
                //offSet.x = rayCastPoint.origin.x - realplayer.transform.position.x;
                //offSet.z = rayCastPoint.origin.y - realplayer.transform.position.y;
                UnityEngine.Debug.Log("offSet: " + offSet.z);
                realplayer.transform.localPosition += offSet;
                    }
                }
            } 
            else if (TouchScreenInputWrapper.touches[0].phase == TouchPhase.Stationary)
            {
                touched = false;
                if (Physics.Raycast(rayCastPoint, out hit))
                {
                    if (hit.transform.gameObject.tag != "Player")
                    {
                Vector3 offSet = new Vector3(0f,0f,0f);
                Vector3 eA = realplayer.transform.eulerAngles;
                float rad = (Mathf.PI / 180f) * (eA.y);
                offSet.x = Mathf.Sin(rad);
                offSet.z = Mathf.Cos(rad);
                //offSet.x = rayCastPoint.origin.x - realplayer.transform.position.x;
                //offSet.z = rayCastPoint.origin.y - realplayer.transform.position.y;
                UnityEngine.Debug.Log("offSet: " + offSet.z);
                realplayer.transform.localPosition += offSet;
                    }
                }
            }          
           
        }

    }
}