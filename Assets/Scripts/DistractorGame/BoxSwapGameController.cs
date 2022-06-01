using UnityEngine;
using System.Collections;

public class BoxSwapGameController : MonoBehaviour {
	public BoxSwapper boxSwapper;

	public GameObject GameContents;
	public GameObject GameBackground;

	float timeBeforeLoweringBoxes = 0.75f;

	void Awake(){
		GameContents.SetActive(false);
	}

	// Use this for initialization
	void Start () {
		//StartCoroutine(RunGame());
	}
	
	public IEnumerator RunGame(){
		PlayerMotion.ControlPause = true;

		Enable (true);

		boxSwapper.Init();

		Color gameBackgroundColor = GameBackground.GetComponent<Renderer> ().material.color;
		Color gameBackgroundColorTransparent = new Color (gameBackgroundColor.r, gameBackgroundColor.g, gameBackgroundColor.b, 0.0f);
		ColorChanger backgroundColorChanger = GameBackground.GetComponent<ColorChanger> ();
		backgroundColorChanger.ChangeColor (gameBackgroundColorTransparent);
		StartCoroutine(backgroundColorChanger.LerpChangeColor (gameBackgroundColor, 0.4f));
		Box1.isenabled = true;
		Box2.isenabled = true;
		Box3.isenabled = true;
		yield return new WaitForSeconds(timeBeforeLoweringBoxes);

		yield return StartCoroutine(boxSwapper.RaiseOrLowerBoxes(-1, false));
		yield return StartCoroutine(boxSwapper.SwapBoxes(4));

		yield return StartCoroutine(boxSwapper.WaitForBoxSelection());

		yield return StartCoroutine(boxSwapper.RaiseOrLowerBoxes(1, true));

		float raisePause = 0.75f;
		yield return new WaitForSeconds (raisePause);
		Box1.isenabled = false;
		Box2.isenabled = false;
		Box3.isenabled = false;
		Enable (false);
		PlayerMotion.ControlPause = false;
	}

	void Enable(bool shouldEnable){
		GetComponent<EnableChildrenLogTrack>().LogChildrenEnabled(shouldEnable);

		//DON'T NEED TO USE USEFULFUNCTIONS.ENABLECHILDREN BECAUSE OF THE 'CONTENTS' SETUP.
		//...but it will be used for replay.
		//UsefulFunctions.EnableChildren( transform, shouldEnable );

		GameContents.SetActive(shouldEnable);
	}
}
