using UnityEngine;
using System.Collections;

public class TriggerFiring : MonoBehaviour {

	GameManager GM;

	// Use this for initialization
	void Start () {
		GM = GameManager.Instance;
	
	}

	// Update is called once per frame
	void Update () {
		//this if statement will be replaced by the lever trigger.
		if(Input.GetKeyDown(KeyCode.D)){
			GM.SetGameState(GameState.Firing);
		}
	}
}
