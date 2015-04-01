using UnityEngine;
using System.Collections;

public class LeverToFire : MonoBehaviour {

	public AudioSource grow;
	public AudioSource hover;


	GameManager GM;

	// Use this for initialization
	void Start () {
		GM = GameManager.Instance;
	
	}

	void OnTriggerEnter(Collider other){

		GetComponent<Animator> ().SetBool ("Bounce", true);
		if (GM.gameState == GameState.Drawing) {
			
			GM.SetGameState (GameState.Firing);
		}
	}

	void OnTriggerExit(Collider other){
		GetComponent<Animator> ().SetBool ("Bounce", false);
	}

	void PlayGrowSound(){
		this.grow.Play();
	}
	
	void PlayHoverSound(){
		if (!this.hover.isPlaying) {
			this.hover.Play ();
		}
	}
	
	void PlayShrinkSound(){
		if (this.hover.isPlaying) {
			this.hover.Stop();
		}
		this.grow.Play ();
	}
}
