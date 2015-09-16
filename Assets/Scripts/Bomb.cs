using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour {

	public ParticleSystem FuseParticles;
	public EllipsoidParticleEmitter ExplosionParticles;

	float totalTravelTime = 2.0f;
	Rigidbody myRigidbody;

	bool isInAir = false;

	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody> ();
		FuseParticles.Stop();

	}
	
	void Update(){
		if (Input.GetKey (KeyCode.B)) {
			StartCoroutine ( ThrowSelf(Vector3.zero, new Vector3(2, 0, 2)) );
			FuseParticles.Play();
		}
	}


	IEnumerator ThrowSelf (Vector3 startPos, Vector3 endPos) {

		Vector3 totalDistance = endPos - startPos;
		//numComponents = (int) (totalDistance.magnitude / 4 );
		
		Vector3 acceleration = Physics.gravity;
		Vector3 initVelocity = (totalDistance - (acceleration * totalTravelTime * totalTravelTime)) / totalTravelTime;

		transform.position = startPos;

		//myRigidbody.velocity = initVelocity;

		float currentTime = 0;

		while (currentTime < totalTravelTime) {
			currentTime += Time.deltaTime;

			Vector3 nextPosition = startPos + (initVelocity * currentTime) + (acceleration * currentTime * currentTime);

			transform.position = nextPosition;

			yield return 0;
		}

		FuseParticles.Stop();
		Instantiate(ExplosionParticles, transform.position, transform.rotation);



		/*float timeStep = totalTravelTime / numComponents;
		for (int i = 0; i < numComponents; i++) {
			float currentTime = timeStep * i;
			Vector3 componentPosition = arcStartPos + (initVelocity * currentTime) + (acceleration * currentTime * currentTime);
			
			ArcComponents [i].transform.position = componentPosition;
		}*/


	}
}
