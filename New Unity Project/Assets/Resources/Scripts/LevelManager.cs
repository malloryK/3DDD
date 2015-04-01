using UnityEngine;
using System.Collections;


public class LevelManager : MonoBehaviour {

	public event OnStateChangeHandler OnLevelChange;

	private static LevelManager _instance =	null;    
	//public event OnStateChangeHandler OnLevelChange;
	GameObject CurrentLevelGO ;
	GameObject rotatingGOs;
	GameObject drawingBox;
	public int currentLevel = 0;

	protected LevelManager() {}


	// Singleton pattern implementation
	public static LevelManager Instance { 
		get {
			if (_instance == null) {
			
				_instance = FindObjectOfType<LevelManager>(); 
				_instance.OnInitiation();
			}  
			return _instance;
		} 
	}

	private void OnInitiation(){
		_instance.rotatingGOs = GameObject.FindGameObjectWithTag ("Moving");
		_instance.drawingBox = GameObject.FindGameObjectWithTag ("DrawBox");
		_instance.CurrentLevelGO = GameObject.Find ("LevelPiece0");
	}

	public void ChangeLevel(int levelNum){


			_instance.rotatingGOs.transform.position = new Vector3 (0, 0, 0);

			Destroy (_instance.CurrentLevelGO);
			_instance.CurrentLevelGO = Instantiate (Resources.Load ("LevelGOs/LevelPiece" + levelNum.ToString (), typeof(GameObject))) as GameObject;
		_instance.CurrentLevelGO.transform.rotation = _instance.rotatingGOs.transform.rotation;
			_instance.CurrentLevelGO.transform.parent = _instance.rotatingGOs.transform;
			_instance.currentLevel = levelNum;
			GameManager.Instance.SetGameState (GameState.Drawing);
			if (OnLevelChange != null) {
				OnLevelChange();
			}

	}

}
