using UnityEngine;
using System.Collections;

public class LeverToFire : MonoBehaviour {


	GameManager GM;

	// Use this for initialization
	void Start () {
		GM = GameManager.Instance;
	
	}

	// Update is called once per frame
	void Update () {
		//this if statement will be replaced by the lever trigger.
		if(Input.GetKeyDown(KeyCode.D)&& GM.gameState != GameState.Firing){

			GM.SetGameState(GameState.Firing);
		}
	}
}
