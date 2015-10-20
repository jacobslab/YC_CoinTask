using UnityEngine;
using System.Collections;

public class BoxSwapGameController : MonoBehaviour {
	public BoxSwapper boxSwapper;

	public GameObject GameContents;

	float timeBeforeLoweringBoxes = 1.0f;

	void Awake(){
		GameContents.SetActive(false);
	}

	// Use this for initialization
	void Start () {
		//StartCoroutine(RunGame());
	}
	
	public IEnumerator RunGame(){
		Enable (true);

		boxSwapper.Init();

		yield return new WaitForSeconds(timeBeforeLoweringBoxes);

		yield return StartCoroutine(boxSwapper.LowerBoxes());
		yield return StartCoroutine(boxSwapper.SwapBoxes(5));

		yield return StartCoroutine(boxSwapper.WaitForBoxSelection());

		yield return StartCoroutine(boxSwapper.RaiseBoxes());

		Enable (false);
	}

	void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);

		//DON'T NEED TO USE USEFULFUNCTIONS.ENABLECHILDREN BECAUSE OF THE 'CONTENTS' SETUP.
		//...but it will be used for replay.
		//UsefulFunctions.EnableChildren( transform, shouldEnable );

		GameContents.SetActive(shouldEnable);
	}
}
