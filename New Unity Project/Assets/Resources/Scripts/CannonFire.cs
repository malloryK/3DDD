using UnityEngine;
using System.Collections;

public class CannonFire : MonoBehaviour {
	public ParticleSystem particles;
	public float FirePower = 1.6f;
	public float DelayInSec = 1;
	public bool fired = false;
	public AudioSource fire;

	GameManager GM;

	// Use this for initialization
	void Start () {
		GM = GameManager.Instance;
		GM.OnStateChange += HandleOnStateChange;
		this.rigidbody.useGravity = false;
	}

	void OnDestroy () {
		GM.OnStateChange -= HandleOnStateChange;
	}

	void HandleOnStateChange(){

		if(GM.gameState == GameState.Firing ){
			StartCoroutine(WaitAndFire());
		}
	}

	IEnumerator WaitAndFire(){

			yield return new WaitForSeconds (DelayInSec);
			this.rigidbody.useGravity = true;
			this.fire.Play ();
			this.rigidbody.AddForce (transform.forward * FirePower, ForceMode.Impulse);
			fired = true;
			particles.Play ();

	}


}
