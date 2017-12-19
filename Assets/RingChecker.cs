using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingChecker : MonoBehaviour {

	public Transform sphereWalls;
	float sphereRadius=48.4f;
	public LayerMask mask;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
//		bool result=CheckInsideSphere (sphereWalls.transform.position, 48.4f, mask.value);

	}

	public bool CheckInsideSphere()
	{
		if (Physics.CheckSphere (sphereWalls.transform.position,sphereRadius, mask.value)) {
			return true;
		} else {
			return false;
		}
	}


}
