using UnityEngine;
using System.Collections;

public class LevelSwitch : MonoBehaviour {

	public GameObject CurrentLevelGO;

	GameObject rotatingGOs;
	GameObject drawingBox;

	void Start(){
		rotatingGOs = GameObject.FindGameObjectWithTag ("Moving");
		drawingBox = GameObject.FindGameObjectWithTag ("DrawBox");
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.Alpha1)){
			SwitchLevels(1);
		}else if(Input.GetKeyDown(KeyCode.Alpha0)){
			SwitchLevels (0);
		}
	}

	void SwitchLevels(int levelNumber){
		//rotatingGOs.transform.position = new Vector3 (0,0,0);
		//drawingBox.activeSelf = true;
		Destroy (CurrentLevelGO);
		CurrentLevelGO = Instantiate(Resources.Load("LevelGOs/LevelPiece"+levelNumber.ToString(), typeof(GameObject))) as GameObject;
		CurrentLevelGO.transform.parent = rotatingGOs.transform;

	}




}
