using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {
	public int LevelToLoad;
	public AudioSource hover;
	public AudioSource grow;

	void Start(){
		LevelManager.Instance.OnLevelChange += HandleOnLevelChange;
	}

	void HandleOnLevelChange ()
	{
		if (LevelManager.Instance.currentLevel == LevelToLoad) {
			this.GetComponent<Light> ().enabled = true;
		} else {
			this.GetComponent<Light>().enabled = false;
		}
	}


	void OnTriggerEnter(Collider other){
		this.GetComponent<Animator>().SetBool ("Bounce",true);
		if(LevelManager.Instance.currentLevel != LevelToLoad || GameManager.Instance.gameState != GameState.Drawing){
			
			LevelManager.Instance.ChangeLevel (LevelToLoad);
		}
	}

	void OnTriggerExit(Collider other){
		this.GetComponent<Animator> ().SetBool ("Bounce",false);
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
