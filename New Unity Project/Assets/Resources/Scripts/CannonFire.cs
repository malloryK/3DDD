using UnityEngine;
using System.Collections;

public class CannonFire : MonoBehaviour {
	public ParticleSystem particles;
	public float FIRE_POWER = 1.6f;
	bool firing = false;
	bool hasFired = false;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyUp ("space") == true && hasFired == false) {
			firing = true;
			hasFired = true;
		}
	
		if (firing) {

			StartCoroutine(WaitAndFire());
			firing = false;

		}
	}

	IEnumerator WaitAndFire(){

		yield return new WaitForSeconds(2.0f);
		this.rigidbody.AddForce(transform.forward * FIRE_POWER, ForceMode.Impulse);

		particles.Play ();

	}
}
