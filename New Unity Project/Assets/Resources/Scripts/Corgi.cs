using UnityEngine;
using System.Collections;

public class Corgi : MonoBehaviour {
	GameManager GM;
	public AudioSource punch;
	public AudioSource win;
	public AudioSource lose;
	public AudioSource jump;



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

		}else if(GM.gameState == GameState.Drawing){
			animation.Play("0_idle");
		}
	}

	IEnumerator PlayWin(){
		GameObject particles = this.gameObject.transform.FindChild ("Hips/Particle System").gameObject;
		animation.Play ("2_Run");
		particles.SetActive(true);
		yield return new WaitForSeconds (.25f);
		jump.Play ();
		animation.Play ("3_Jump");
		yield return new WaitForSeconds (.55f);
		animation.Play ("3_Jump");
		win.Play ();

	}

	IEnumerator PlayLose(){

		animation.Play ("5_Pain");
		yield return new WaitForSeconds (.6f);
		lose.Play ();
		animation.Play ("6_Dead");

	}

	void OnTriggerEnter(Collider other){
		if (!other.CompareTag ("Stand")) {
			punch.Play();
			GM.SetGameState(GameState.Lose);
			print ("lose");
		}
	}



}
