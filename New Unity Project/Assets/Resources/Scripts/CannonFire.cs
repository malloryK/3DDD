using UnityEngine;
using System.Collections;

public class CannonFire : MonoBehaviour {

	bool firing = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown ("space")) {
			firing = true;
		}
	
		if (firing) {

			StartCoroutine(WaitAndFire());
			firing = false;

			//do firing
			//this.rigidbody.AddForce(transform.forward * 12, ForceMode.Impulse);
			//firing = false;


		}
	}

	IEnumerator WaitAndFire(){

		yield return new WaitForSeconds(2.0f);
		this.rigidbody.AddForce(transform.forward * 12, ForceMode.Impulse);

	}
}
