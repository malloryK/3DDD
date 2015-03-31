using UnityEngine;
using System.Collections;

public class LevelGameObjects : MonoBehaviour {
	public CannonFire[] CannonBalls;
	GameManager GM ;

	void Start(){
		GM = GameManager.Instance;
		GM.OnStateChange+= HandleOnStateChange;
	}

	void HandleOnStateChange(){
		if (GM.gameState == GameState.Firing) {
			StartCoroutine (CheckWin ());
		}
	}

	void OnDestroy()
	{
		GM.OnStateChange -= HandleOnStateChange;
	}
	
	IEnumerator CheckWin(){
		bool cannonsDone = false;

		while(!cannonsDone){
			cannonsDone= true;
			foreach(CannonFire cannon in CannonBalls){
				if(cannon.fired == false){
					cannonsDone = false;
				}
			}
			yield return null;
		}
		yield return new WaitForSeconds (3.5f);
		if(GM.gameState != GameState.Lose){
			GM.SetGameState(GameState.Win);
		}
		yield return null;
			
			
	}

}
