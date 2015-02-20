using UnityEngine;
using System.Collections;

public class UndoScript : MonoBehaviour {


	public float spring;
	public float triggerDistance;

	private Vector3 resting_position_;
	DrawMesh meshDrawer ;

	public void CheckTrigger(){
		if(transform.localPosition.z > triggerDistance){
			meshDrawer.UndoAction();

		}
	}

	protected void ApplySpring(){
		rigidbody.AddRelativeForce (  -spring * (transform.localPosition -resting_position_));
	}

	// Use this for initialization
	void Start () {
		GameObject MeshDrawer = GameObject.FindGameObjectWithTag("MeshDrawer") as GameObject;
		meshDrawer = MeshDrawer.GetComponent<DrawMesh> ();
		resting_position_ = transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		ApplySpring ();
		CheckTrigger ();
	}
}
