using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingChecker : MonoBehaviour {

	public SphereCollider sphereCollider;
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
		if (Physics.CheckSphere (sphereCollider.transform.position,sphereCollider.radius, mask.value)) {
			return true;
		} else {
			return false;
		}
	}


}
