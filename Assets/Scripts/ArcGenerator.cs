using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArcGenerator : MonoBehaviour {

	public GameObject ArcComponent;

	public float totalArcTime;
	public int numComponents;
	List<GameObject> ArcComponents;

	// Use this for initialization
	void Start () {
		ArcComponents = new List<GameObject>();
		GenerateArcComponents ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void GenerateArcComponents(){
		for (int i = 0; i < numComponents; i++) {
			GameObject newArcComponent = Instantiate (ArcComponent, transform.position, Quaternion.identity) as GameObject;
			ArcComponents.Add (newArcComponent);
			newArcComponent.transform.parent = transform;
		}
	}

	public void AdjustArc(Vector3 arcStartPos, Vector3 arcEndPos){

		Vector3 totalDistance = arcEndPos - arcStartPos;
		//numComponents = (int) (totalDistance.magnitude / 4 );

		Vector3 acceleration = Physics.gravity;
		Vector3 initVelocity = (totalDistance - (acceleration*totalArcTime*totalArcTime) ) / totalArcTime;

		float timeStep = totalArcTime / numComponents;
		LineRenderer arcLine = GetComponent<LineRenderer> ();
		for(int i = 0; i < numComponents; i++){
			float currentTime = timeStep*i;
			Vector3 nextPosition = arcStartPos + (initVelocity * currentTime) + ( acceleration * currentTime * currentTime );

			/*ArcComponents[i].transform.position = nextPosition;
			if(i > 0){
				ArcComponents[i].transform.LookAt( ArcComponents [i - 1].transform );
			}*/
			arcLine.SetPosition(i, nextPosition);
		}

		//set the last position
		arcLine.SetPosition (numComponents, arcEndPos);
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
