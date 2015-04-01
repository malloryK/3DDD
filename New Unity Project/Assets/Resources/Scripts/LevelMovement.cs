using UnityEngine;
using System.Collections;

public class LevelMovement : MonoBehaviour {
	GameManager GM;
	float moveTime = 0;

	void Start () {
		GM = GameManager.Instance;
		GM.OnStateChange += HandleOnStateChange;
	}

	void HandleOnStateChange(){
		if (GM.gameState == GameState.Firing) {
			moveTime = Time.time + 1;//rise for 3 secs
			StartCoroutine(LevelRise());
		}else if (GM.gameState == GameState.Drawing){
			transform.rotation = Quaternion.identity;
		}
	}
	

	IEnumerator LevelRise(){
		while(Time.time<moveTime){
			transform.Translate (Vector3.up * Time.deltaTime * 1, Space.World);
			yield return new WaitForEndOfFrame();
		}
	}


}
