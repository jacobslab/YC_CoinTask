using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcGenerator : MonoBehaviour {

	public GameObject ArcComponent;

	public float totalArcTime;
	int numComponents = 10;
	List<GameObject> ArcComponents;

	// Use this for initialization
	void Start () {
		ArcComponents = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void GenerateArc(Vector3 arcToPosition){
		DeleteArc();

		Vector3 arcStartingPos = transform.position;

		Vector3 totalDistance = arcToPosition - arcStartingPos;
		Vector3 acceleration = Physics.gravity;
		Vector3 initVelocity = (totalDistance - (acceleration*totalArcTime*totalArcTime) ) / totalArcTime;

		float timeStep = totalArcTime / numComponents;
		for(int i = 0; i < numComponents; i++){
			float currentTime = timeStep*i;
			Vector3 componentPosition = arcStartingPos + (initVelocity * currentTime) + ( acceleration * currentTime * currentTime );

			GameObject newArcComponent = Instantiate(ArcComponent, componentPosition, Quaternion.identity) as GameObject;
			ArcComponents.Add(newArcComponent);
			newArcComponent.transform.parent = transform;
		}
	}

	public void DeleteArc(){
		int arcCount = ArcComponents.Count;
		for(int i = 0; i < arcCount; i++){
			//always remove the first item because we keep removing items from the list!
			Destroy(ArcComponents[0]);
			ArcComponents.RemoveAt(0);
		}
	}
}
