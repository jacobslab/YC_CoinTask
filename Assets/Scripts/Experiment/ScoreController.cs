using UnityEngine;
using System.Collections;

public class ScoreController : MonoBehaviour {
	
	Experiment_CoinTask exp { get { return Experiment_CoinTask.Instance; } }

	public int score = 0;

	/*public float minRange = 10;
	public float midRange = 30;
	public float maxRange = 50;*/


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public int CalculatePoints(GameObject desiredObject){
		Vector2 avatarXYPos = new Vector2( exp.avatar.transform.position.x, exp.avatar.transform.position.z);
		Vector2 objectXYPos = new Vector2( desiredObject.transform.position.x, desiredObject.transform.position.z);

		float distanceFromObject = (avatarXYPos - objectXYPos).magnitude;

		//calculate point ranges based on the visual circles from the map
		float minRange = exp.environmentMap.SmallScoreRing.GetComponent<Renderer>().bounds.extents.x;
		//Debug.Log("min range: " + minRange);
		float midRange = exp.environmentMap.MediumScoreRing.GetComponent<Renderer>().bounds.extents.x;
		//Debug.Log("mid range: " + midRange);
		float maxRange = exp.environmentMap.BigScoreRing.GetComponent<Renderer>().bounds.extents.x;
		//Debug.Log("max range: " + maxRange);

		if(distanceFromObject < minRange){
			score += 100;
			return 100;
		}
		else if(distanceFromObject < midRange){
			score += 50;
			return 50;
		}
		else if(distanceFromObject < maxRange){
			score += 25;
			return 25;
		}
		else{
			score += 10;
			return 10;
		}
	}
}
