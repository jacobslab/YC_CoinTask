using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriSpawn : MonoBehaviour {

    public GameObject cubeA;
    public GameObject cubeB;

    public GameObject cubeC;

    public float angle = 36f;
    public float radius = 5f;
	// Use this for initialization
	void Start () {
                                    //Vector3 vec= Vector3.Cross(v, Vector3.up);
                                    //Debug.Log("cross product vec " + vec.ToString());



        //float dot = Vector3.Dot(v, transform.forward);
        //float dotAngle = Mathf.Acos( dot ) * Mathf.Rad2Deg;

        //Debug.Log("dot angle is " + dotAngle.ToString());
        ////Vector3 vec = Vector3.ProjectOnPlane(cubeB.transform.position-cubeA.transform.position, cubeB.transform.position);
        //cubeC.transform.position = vec + cubeB.transform.position;

        //Vector3 g = cubeC.transform.position - cubeB.transform.position;

        //float angle = Vector3.Angle(v, g);
        //Debug.Log("angle is " + angle.ToString());

    }
	
	// Update is called once per frame
    void Update () {
        //Vector3 spawnPoint;
        float spawnDistance = radius;
        Vector3 v = cubeB.transform.position - cubeA.transform.position;

        //Vector3 dir = Vector3.Cross(v.normalized, Vector3.up);
        Vector3 dir = new Vector3(Mathf.Sin(Mathf.Deg2Rad * angle), 0f, Mathf.Cos(Mathf.Deg2Rad * angle));
        //dir = Vector3.Cross(dir, Vector3.up);
        Debug.Log("dir " + dir.ToString());
        cubeC.transform.position = cubeB.transform.position + (dir * radius);
        ////Vector2 vec = (new Vector2(-v.y, v.x) / Mathf.Sqrt(Mathf.Pow(v.x,2) + Mathf.Pow(v.y, 2))) * 5f;
        //Debug.Log("v is " + v.ToString());

        //var degrees = angle;
        //var radians = degrees * Mathf.Deg2Rad;
        //var x =  cubeB.transform.position *Mathf.Cos(radians);
        //var y = cubeB.transform.position * Mathf.Sin(radians);
        //Debug.Log("Cos " + x.ToString());
        //Debug.Log("Sine " + y.ToString());
        //var pos = new Vector3(x, y, 0); //Vector2 is fine, if you're in 2D
        //pos = pos * radius;
        //spawnPoint = new Vector3(x, y, 0) * spawnDistance;
        //spawnPoint = cubeB.transform.position + spawnPoint;
        //cubeC.transform.position = spawnPoint;
    }
}
