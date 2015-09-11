using UnityEngine;
using System.Collections;

public class EnvironmentController : MonoBehaviour {
	public Transform WallsXPos;
	public Transform WallsXNeg;
	public Transform WallsZPos;
	public Transform WallsZNeg;

	public Vector3 center{ get { return GetEnvironmentCenter(); } }

	public EnvironmentGrid myGrid;
	public EnvironmentPositionSelector myPositionSelector;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector3 GetEnvironmentCenter(){
		float centerX = (WallsXPos.position.x + WallsXNeg.position.x + WallsZNeg.position.x + WallsZPos.position.x) / 4.0f;
		float centerZ = (WallsXPos.position.z + WallsXNeg.position.z + WallsZNeg.position.z + WallsZPos.position.z) / 4.0f;

		return new Vector3(centerX, 0.0f, centerZ);
	}

	public bool CheckWithinWallsHoriz(Vector3 position, float wallBuffer){
		if(position.x < WallsXPos.position.x - wallBuffer && position.x > WallsXNeg.position.x + wallBuffer){
			return true;
		}

		return false;
	}

	public bool CheckWithinWallsVert(Vector3 position, float wallBuffer){
		if(position.z < WallsZPos.position.z - wallBuffer && position.z > WallsZNeg.position.z + wallBuffer){
			return true;	
		}
		
		return false;
	}

	public Vector3 GetRandomPositionWithinWallsXZ(float wallBuffer){
		
		float randomXPos = Random.Range(WallsXPos.position.x - wallBuffer, WallsXNeg.position.x + wallBuffer);
		float randomZPos = Random.Range(WallsZPos.position.z - wallBuffer, WallsZNeg.position.z + wallBuffer);
		
		Vector3 newPosition = new Vector3 (randomXPos, transform.position.y, randomZPos);

		
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



		/*Vector3 xPosDistance = WallsXPos.transform.position - position;
		Vector3 xNegDistance = WallsXNeg.transform.position - position;
		Vector3 zPosDistance = WallsZPos.transform.position - position;
		Vector3 zNegDistance = WallsZNeg.transform.position - position;

		float angleXPos = Mathf.Atan2 (xPosDistance.x , xPosDistance.z) * 180.0f / 3.14159f;//(xPosDistance.z / xPosDistance.x) * 180.0f / 3.14159f;
		float angleXNeg = Mathf.Atan2 (xNegDistance.x , xNegDistance.z) * 180.0f / 3.14159f;//(xNegDistance.z / xNegDistance.x) * 180.0f / 3.14159f;
		float angleZPos = Mathf.Atan2 (zPosDistance.x , zPosDistance.z) * 180.0f / 3.14159f;//(zPosDistance.z / zPosDistance.x) * 180.0f / 3.14159f;
		float angleZNeg = Mathf.Atan2 (zNegDistance.x , zNegDistance.z) * 180.0f / 3.14159f;//(zNegDistance.z / zNegDistance.x) * 180.0f / 3.14159f;

		if (angleXPos < angleXNeg && angleXPos < angleZPos && angleXPos < angleZNeg) { //xPos wall has the smallest angle difference
			distance = (WallsXPos.position.x + wallBuffer) - position.x;
		}
		else if (angleXNeg < angleZPos && angleXNeg < angleZNeg){ //xNeg wall has the smallest angle difference
			distance = (WallsXNeg.position.x - wallBuffer) - position.x;
		}
		else if (angleZPos < angleZNeg){ //zPos wall has the smallest angle difference
			distance = (WallsZPos.position.z - wallBuffer) - position.z;
		}
		else{ //zNeg wall has the smallest angle difference
			distance = (WallsZNeg.position.z + wallBuffer) - position.z;
		}*/

		return distanceToWall;
	}

}
