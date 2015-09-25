using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

[RequireComponent (typeof (VisibilityToggler))]
[RequireComponent (typeof (ObjectLogTrack))]
public class SpawnableObject : MonoBehaviour {

	VisibilityToggler myVisibilityToggler;
	public bool isVisible { get { return myVisibilityToggler.GetVisibility (); } }

	ObjectLogTrack myLogTrack;

	Vector3 origScale;

	// Use this for initialization
	void Awake () {
		myVisibilityToggler = GetComponent<VisibilityToggler> ();
		myLogTrack = GetComponent<ObjectLogTrack> ();
		origScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//function to turn off (or on) the object without setting it inactive -- because we want to keep logging on
	public void TurnVisible(bool shouldBeVisible){ 
		if (myVisibilityToggler == null) {
			myVisibilityToggler = GetComponent<VisibilityToggler> ();
		}
		myVisibilityToggler.TurnVisible (shouldBeVisible);
	}

	public string GetName(){
		string name = gameObject.name;
		name = Regex.Replace( name, "(Clone)", "" );
		name = Regex.Replace( name, "[()]", "" );

		return name;
	}

	//should be set when spawned by the ObjectController
	public void SetNameID(int ID){
		if (ID < 10) {
			gameObject.name = GetName() + "00" + ID; 
		}
		else if(ID < 100) {
			gameObject.name = GetName() + "0" + ID; 
		}
		else if(ID < 1000) {
			gameObject.name = GetName() + ID; 
		}
	}

	public void Scale(float scaleMult){
		transform.localScale *= scaleMult;
	}

	public void SetOrigScale(){
		transform.localScale = origScale;
	}

	public void SetLayer(string newLayer){
		UsefulFunctions.SetLayerRecursively (gameObject, newLayer);

		myLogTrack.LogLayerChange ();
	}

	public void SetShadowCasting(bool shouldCastShadows){
		UnityEngine.Rendering.ShadowCastingMode shadowMode = UnityEngine.Rendering.ShadowCastingMode.On;
		if (!shouldCastShadows) {
			shadowMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		}

		if(GetComponent<Renderer>() != null){
			GetComponent<Renderer>().shadowCastingMode = shadowMode;
		}
		
		Renderer[] renderers = GetComponentsInChildren<Renderer>();
		for(int i = 0; i < renderers.Length; i++){
			renderers[i].shadowCastingMode = shadowMode;
		}

		myLogTrack.LogShadowSettings (shadowMode);
	}

}