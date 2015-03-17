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
			moveTime = Time.time + 5;//rise for 3 secs
			StartCoroutine(LevelRise());
		}
	}

	void Update(){
		if (moveTime != 0 && Time.time > moveTime) {
			print (Time.time + "and" + moveTime);
			StopCoroutine("LevelRise");
		}
	}

	IEnumerator LevelRise(){
		print ("rise" + Time.time);
		transform.Translate (Vector3.up * Time.deltaTime * 10, Space.World);
		yield return new WaitForEndOfFrame();
	}


}
