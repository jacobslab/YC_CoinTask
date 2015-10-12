using UnityEngine;
using System.Collections;

public class Explosive : MonoBehaviour {

	//public ParticleSystem FuseParticles;
	//public EllipsoidParticleEmitter ExplosionParticleEmitter;
	public ParticleSystem ExplosionParticles;
	float particleHeightOffset = 4.0f;
	//public AudioSource FuseSound;

	float totalTravelTime = 1.2f;
	Rigidbody myRigidbody;

	// Use this for initialization
	void Start () {

	}
	
	void Update(){

	}


	public IEnumerator ThrowSelf (Vector3 startPos, Vector3 endPos) {
		/*if (FuseParticles != null) {
			FuseParticles.Play ();
		}*/

		myRigidbody = GetComponent<Rigidbody> ();

		float randomTorqueX = Random.Range (-10.0f, 10.0f);
		float randomTorquey = Random.Range (-40.0f, 40.0f);
		float randomTorqueZ = Random.Range (-10.0f, 10.0f);
		myRigidbody.AddTorque (randomTorqueX, 0.0f, randomTorqueZ);

		Vector3 totalDistance = endPos - startPos;
		
		Vector3 acceleration = Physics.gravity;
		Vector3 initVelocity = (totalDistance - (acceleration * totalTravelTime * totalTravelTime)) / totalTravelTime;

		transform.position = startPos;

		float currentTime = 0;

		while (currentTime < totalTravelTime) {
			currentTime += Time.deltaTime;

			Vector3 nextPosition = startPos + (initVelocity * currentTime) + (acceleration * currentTime * currentTime);

			transform.position = nextPosition;

			yield return 0;
		}

		/*if (FuseParticles != null) {
			FuseParticles.Stop ();
		}*/
		ParticleSystem explosionParticles = Instantiate(ExplosionParticles, transform.position + Vector3.up*particleHeightOffset, transform.rotation) as ParticleSystem;
		GetComponent<SpawnableObject> ().TurnVisible (false);
		while (explosionParticles.isPlaying) {
			yield return 0;
		}


		Destroy (gameObject);
	}
}
