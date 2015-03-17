using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour {
	
	public GameObject drawingBox;

	bool drawingState = true;

	// Use this for initialization
	void Start () {
		Physics.gravity = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown(KeyCode.D)){
			Physics.gravity = new Vector3 (0,-1,0);
			//TODO: drawingBox.SetActive(false);
		}
	}



}
