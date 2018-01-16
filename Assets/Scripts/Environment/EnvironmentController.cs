using UnityEngine;
using System.Collections;

public class EnvironmentController : MonoBehaviour {
	public Transform WallsXPos;
	public Transform WallsXNeg;
	public Transform WallsZPos;
	public Transform WallsZNeg;

	public Transform arenaCenter;
	public SphereCollider sphereWalls;

	public float xFactor=1f;
	public float zFactor=1f;


	public GameObject HomeBaseIndicatorA;
	public GameObject HomeBaseIndicatorB;

	public Vector3 center{ get { return GetEnvironmentCenter(); } }
	public EnvironmentPositionSelector myPositionSelector;


	// Use this for initialization
	void Start () {
		Debug.Log ("wallsxpos: " + WallsXPos.position.ToString());
		Debug.Log ("wallsxneg: " + WallsXNeg.position.ToString ());
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public static void SetSkybox(Material skyboxMat){
		RenderSettings.skybox = skyboxMat;
	}

	public Vector3 GetEnvironmentCenter(){
//		float centerX = (WallsXPos.position.x + WallsXNeg.position.x + WallsZNeg.position.x + WallsZPos.position.x) / 4.0f;
//		float centerZ = (WallsXPos.position.z + WallsXNeg.position.z + WallsZNeg.position.z + WallsZPos.position.z) / 4.0f;
//		Debug.Log ("center is: " + centerX.ToString () + " ,0f, " + centerZ.ToString ());
		return arenaCenter.transform.position;
	}


	public bool CheckWithinWallsHoriz(Vector3 position, float wallBuffer){
//		Debug.Log ("pos x " + position.x.ToString());
//		Debug.Log ("arena center pos x: " + (arenaCenter.position.x + sphereWalls.radius - wallBuffer).ToString());
//		Debug.Log ("arena center neg x: " + (arenaCenter.position.x - sphereWalls.radius +wallBuffer).ToString());
		if (position.x < arenaCenter.position.x + sphereWalls.radius - wallBuffer&& position.x > arenaCenter.position.x - sphereWalls.radius + wallBuffer) {
			return true;
		} else
			return false;
//		if(position.x < WallsXPos.position.x - wallBuffer && position.x > WallsXNeg.position.x + wallBuffer){
//			//Debug.Log ("within horiz walls");
//			return true;
//		}
//		//Debug.Log ("NOT within horiz walls");
//		return false;
	}

	public bool CheckWithinWallsVert(Vector3 position, float wallBuffer){
//		Debug.Log ("pos z: " + position.z.ToString ());
//		Debug.Log ("arena center pos z: " + (arenaCenter.position.z + sphereWalls.radius - wallBuffer).ToString());
//		Debug.Log ("arena center neg z: " + (arenaCenter.position.z - sphereWalls.radius +wallBuffer).ToString());

		if (position.z < arenaCenter.position.z + sphereWalls.radius - wallBuffer && position.z > arenaCenter.position.z - sphereWalls.radius + wallBuffer) {
			return true;
		} else
			return false;
//		if(position.z < WallsZPos.position.z - wallBuffer && position.z > WallsZNeg.position.z + wallBuffer){
//			//.Log ("within vert walls");
//			return true;	
//		}
//		//Debug.Log ("NOT within vert walls");
//		return false;
	}

	public Vector3 GetRandomPositionWithinWallsXZ(float wallBuffer){
		
		//float randomXPos = Random.Range(WallsXPos.position.x - wallBuffer, WallsXNeg.position.x + wallBuffer);
		//float randomZPos = Random.Range(WallsZPos.position.z - wallBuffer, WallsZNeg.position.z + wallBuffer);
//		float randomXPos=Random.Range(sphereWalls.transform.position.x - sphereWalls.radius - wallBuffer, sphereWalls.transform.position.x + sphereWalls.radius + wallBuffer);
//		float randomZPos=Random.Range(sphereWalls.transform.position.z - sphereWalls.radius - wallBuffer, sphereWalls.transform.position.z + sphereWalls.radius + wallBuffer);
		//Vector3 newPosition = new Vector3 (randomXPos, transform.position.y, randomZPos);
		Vector3 randSphere=Random.insideUnitSphere;
		UnityEngine.Debug.Log ("randsphere: " + randSphere.ToString ());
		Vector3 newPosition = new Vector3 (sphereWalls.transform.position.x + (randSphere.x * sphereWalls.radius), 
			sphereWalls.transform.position.y, sphereWalls.transform.position.z + (randSphere.z * sphereWalls.radius));
//		UnityEngine.Debug.Log ("new position: " + newPosition);
		return newPosition;
	}

	public Vector3 GetRandomStartPosition(float radiusBuffer){

		//float randomXPos = Random.Range(WallsXPos.position.x - wallBuffer, WallsXNeg.position.x + wallBuffer);
		//float randomZPos = Random.Range(WallsZPos.position.z - wallBuffer, WallsZNeg.position.z + wallBuffer);
		//		float randomXPos=Random.Range(sphereWalls.transform.position.x - sphereWalls.radius - wallBuffer, sphereWalls.transform.position.x + sphereWalls.radius + wallBuffer);
		//		float randomZPos=Random.Range(sphereWalls.transform.position.z - sphereWalls.radius - wallBuffer, sphereWalls.transform.position.z + sphereWalls.radius + wallBuffer);
		//Vector3 newPosition = new Vector3 (randomXPos, transform.position.y, randomZPos);
		Vector3 randSphere=Random.insideUnitSphere;
		UnityEngine.Debug.Log ("randsphere: " + randSphere.ToString ());
		Vector3 newPosition = new Vector3 (sphereWalls.transform.position.x + (randSphere.x * (sphereWalls.radius-radiusBuffer)), 
			sphereWalls.transform.position.y, sphereWalls.transform.position.z + (randSphere.z * (sphereWalls.radius-radiusBuffer)));
		//		UnityEngine.Debug.Log ("new position: " + newPosition);
		return newPosition;
	}
	//NOT WORKING AS INTENDED.
	public float GetDistanceFromEdge(GameObject positionObject, float wallBuffer, Vector3 direction){
		float distanceToWall = 0;

		RaycastHit wallHit;
		
		if (Physics.Raycast(positionObject.transform.position, direction, out wallHit, 100.0F)) {
			if(positionObject.transform.forward == -Vector3.forward){
				if(wallHit.collider.tag == "WallXPos"){
					distanceToWall = WallsXPos.transform.position.x - positionObject.transform.position.x - wallBuffer;
				}
				else if(wallHit.collider.tag == "WallXNeg"){
					distanceToWall = WallsXNeg.transform.position.x - positionObject.transform.position.x + wallBuffer;
				}
				else if(wallHit.collider.tag == "WallZPos"){
					distanceToWall = WallsZPos.transform.position.z - positionObject.transform.position.z - wallBuffer;
				}
				else if(wallHit.collider.tag == "WallZNeg"){
					distanceToWall = WallsZNeg.transform.position.z - positionObject.transform.position.z + wallBuffer;
				}
			}
			else if(positionObject.transform.forward == Vector3.forward){
				if(wallHit.collider.tag == "WallXPos"){
					distanceToWall = WallsXPos.transform.position.x - positionObject.transform.position.x + wallBuffer;
				}
				else if(wallHit.collider.tag == "WallXNeg"){
					distanceToWall = WallsXNeg.transform.position.x - positionObject.transform.position.x - wallBuffer;
				}
				else if(wallHit.collider.tag == "WallZPos"){
					distanceToWall = WallsZPos.transform.position.z - positionObject.transform.position.z + wallBuffer;
				}
				else if(wallHit.collider.tag == "WallZNeg"){
					distanceToWall = WallsZNeg.transform.position.z - positionObject.transform.position.z - wallBuffer;
				}
			}
		}
		Debug.Log ("distance to wall: " + distanceToWall.ToString ());
		return distanceToWall;
	}

	public void EnableHomeBaseIndicators(bool isEnabled){
		HomeBaseIndicatorA.SetActive (isEnabled);
		HomeBaseIndicatorB.SetActive (isEnabled);
	}

}
