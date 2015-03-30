using UnityEngine;
using System.Collections;

public class LoadLevel : MonoBehaviour {
	public int LevelToLoad;

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
		if(LevelManager.Instance.currentLevel != LevelToLoad){
			
			LevelManager.Instance.ChangeLevel (LevelToLoad);
		}
	}

	void OnTriggerExit(Collider other){
		this.GetComponent<Animator> ().SetBool ("Bounce",false);
	}

}
