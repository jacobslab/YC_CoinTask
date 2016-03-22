using UnityEngine;
using System.Collections;

//from http://answers.unity3d.com/questions/353680/mirror-flip-camera.html
[RequireComponent (typeof (Camera))]
public class MirrorFlipCamera : MonoBehaviour {
	Camera myCamera;

	bool shouldFlip = false;

	bool shouldFlipVert = false;
	bool shouldFlipHoriz = true;

	void Awake(){
		myCamera = GetComponent<Camera> ();
#if MRIVERSION
		if(!Config_CoinTask.isPractice){
			shouldFlip = true;
		}
#endif
	}

	void Start(){

	}
	
	void OnPreCull () {
		if (myCamera && shouldFlip) {
			myCamera.ResetWorldToCameraMatrix ();
			myCamera.ResetProjectionMatrix ();
			if(shouldFlipVert && !shouldFlipHoriz){
				myCamera.projectionMatrix = (myCamera.projectionMatrix * Matrix4x4.Scale (new Vector3 (1, -1, 1)));
			}
			else if(shouldFlipHoriz && !shouldFlipVert){
				myCamera.projectionMatrix = myCamera.projectionMatrix * Matrix4x4.Scale (new Vector3 (-1, 1, 1));
			}
			else if(shouldFlipVert && shouldFlipHoriz){
				myCamera.projectionMatrix = myCamera.projectionMatrix * Matrix4x4.Scale (new Vector3 (-1, -1, 1));
			}

		} else {
			Debug.Log("No camera!");
		}
	}
	
	void OnPreRender () {
		if (shouldFlip) {
			if (shouldFlipVert || shouldFlipHoriz) {
				GL.SetRevertBackfacing (true);
			}
		}
	}
	
	void OnPostRender () {
		if (shouldFlip) {
			if (shouldFlipVert || shouldFlipHoriz) {
				GL.SetRevertBackfacing (false);
			}
		}
	}
}
