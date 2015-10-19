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
		GameContents.SetActive(true);

		boxSwapper.Init();

		yield return new WaitForSeconds(timeBeforeLoweringBoxes);

		yield return StartCoroutine(boxSwapper.LowerBoxes());
		yield return StartCoroutine(boxSwapper.SwapBoxes(5));

		yield return StartCoroutine(boxSwapper.WaitForBoxSelection());

		yield return StartCoroutine(boxSwapper.RaiseBoxes());

		GameContents.SetActive(false);
	}
}
