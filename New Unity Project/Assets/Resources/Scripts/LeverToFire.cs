using UnityEngine;
using System.Collections;

public class LeverToFire : MonoBehaviour {


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

	void OnTriggerEnter(){
		GetComponent<Animator> ().SetBool ("Bounce", false);
	}
}
