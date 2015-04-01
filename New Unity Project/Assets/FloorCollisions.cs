using UnityEngine;
using System.Collections;

public class FloorCollisions : MonoBehaviour {
	bool played = false;
	public AudioSource bang;


	void Start(){
		GameManager.Instance.OnStateChange += HandleOnStateChange;
	}

	void HandleOnStateChange ()
	{
		if (GameManager.Instance.gameState == GameState.Drawing) {
			played = false;
		}
	}

	void OnCollisionEnter(Collision other){
		Debug.Log(other.gameObject.name);
		if (other.gameObject.CompareTag ("Voxel") && !played) {
			Debug.Log("landed");
			bang.Play();
			played = true;
		}
	}
}
