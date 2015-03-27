using UnityEngine;
using System.Collections;

public class Corgi : MonoBehaviour {
	GameManager GM;
	// Use this for initialization
	void Start () {
		GM = GameManager.Instance;
		GM.OnStateChange += HandleOnStateChange;
	}

	void HandleOnStateChange ()
	{
		if(GM.gameState == GameState.Lose){
			StartCoroutine(PlayLose());
		}else if(GM.gameState == GameState.Win){
			StartCoroutine(PlayWin());
		}
	}

	IEnumerator PlayWin(){
		animation.Play ("2_Run");
		yield return new WaitForSeconds (.25f);
		animation.Play ("3_Jump");
	}

	IEnumerator PlayLose(){

		animation.Play ("5_Pain");
		yield return new WaitForSeconds (.6f);
		animation.Play ("6_Dead");

	}

	void OnTriggerEnter(Collider other){
		if (!other.CompareTag ("Stand")) {
			GM.SetGameState(GameState.Lose);
			print ("lose");
		}
	}



}
