﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;


public class DrawMesh : MonoBehaviour {
	Mesh mesh;
	Material mat;
	List<Vector3> Points;
	List<Vector3> Verts;
	List<int> Tris;

	Hand rightHand;
	HandModel rightRigid;
	Hand leftHand;
	HandModel leftRigid;
	public GameObject drawBox;
	public GameObject prefabShape;
	GameObject drawnShape;


	GameObject level;


	Vector3 lastMovePinchSpot, lastRotatePinchSpot;

	Quaternion lastRotation;
	Vector3 lastPosition;
	GameObject lastObject;
	
	float CONFIDENCE_TO_DRAW = 0.1f; // confidence needed that hand is pointing to add to mesh
	float CONFIDENCE_TO_GRAB = 0.3f;
	float LEVEL_HEIGHT = 5.5f;
	public HandController handController;
		
	enum LastAction {NONE, DRAW, TRANSLATE, ROTATE};
	LastAction lastAction = LastAction.NONE;

	bool isDrawing = false;


	void Start () {
		Points = new List<Vector3>();
		Verts = new List<Vector3>();
		Tris = new List<int> ();

		//NewShape (Vector3.zero, lastObject);
		level = GameObject.Find("Level");



	}

	void CopyShape(){
//		ArrayList comps= lastObject.GetComponents();
//		for(int i = 0; i<comps.Count-1; i++){
//			Component.Destroy (comps[i]);
//		}
//		ArrayList comps2 = drawnShape.GetComponents ();
//		for(int i = 0; i<comps.Count-1; i++){
//			lastObject.AddComponent(comps2[1]) ;
//		}
//
//		lastObject.SetActive(false);

	}

	void NewShape(Vector3 position, GameObject overWrite){

		Points.Clear ();
		Verts.Clear ();
		Tris.Clear ();
	
		drawnShape =  Instantiate (prefabShape,position , Quaternion.identity ) as GameObject;

		CreateMesh ();
		
		
	}


	void Update () {
		//get hands 
		rightHand = handController.GetFrame().Hands.Rightmost;	
		leftHand = handController.GetFrame ().Hands.Leftmost;
		//check for correct handedness
		Hand temp = null;
		if (!rightHand.IsRight) {
			temp = rightHand;
			rightHand = null;

		} 
		if (!leftHand.IsLeft) {
			rightHand = leftHand;
			if( temp!=null){
				leftHand = temp;
			}else{
				leftHand = null;
			}
		} 

		//recognize gestures of hand and assign tasks
		if(rightHand != null &&  rightHand.Confidence > CONFIDENCE_TO_DRAW){
			//check just the index is pointing

			//drawing
			if ( rightHand.Fingers[0].IsExtended == false && rightHand.Fingers[1].IsExtended == true &&  rightHand.Fingers[2].IsExtended == false && rightHand.Fingers[3].IsExtended == false && rightHand.Fingers[4].IsExtended == false)
			{
				DrawOnIndexPoint();

			}else{
				isDrawing = false;
				// rotating
				 if(leftHand!=null && leftHand.GrabStrength >CONFIDENCE_TO_GRAB && rightHand.GrabStrength>CONFIDENCE_TO_GRAB && drawnShape != null){
					if(rightHand.WristPosition.y > LEVEL_HEIGHT)
					{
						RotateShape(drawnShape.collider.bounds.center);

					}
					else
					{
						RotateShape(level.transform.position);
					}

				}else{
					lastRotatePinchSpot = Vector3.zero;
					//moving
					 if(rightHand.GrabStrength > CONFIDENCE_TO_GRAB && drawnShape != null){
						MoveShape();

					}else {
						lastMovePinchSpot = Vector3.zero;
					}
				}
			}
		}
		if (Input.GetKeyDown ("space")) { 	 	
			if(drawnShape != null){
				drawnShape.rigidbody.useGravity= true;

			}

		}
	}



	void MoveShape ()
	{
		Vector3 pinchSpot = (handController.transform.TransformPoint(rightHand.Fingers[1].TipPosition.ToUnityScaled ()));
		if (lastMovePinchSpot != Vector3.zero) {
			drawnShape.transform.position  += (pinchSpot-lastMovePinchSpot);
			lastAction = LastAction.TRANSLATE;

		}else{
			lastPosition = drawnShape.transform.position;
		}
		lastMovePinchSpot = pinchSpot;
	}

	void RotateShape (Vector3 objectPoint)
	{	

		Vector3 pinchSpot = objectPoint-(handController.transform.TransformPoint(rightHand.Fingers[1].TipPosition.ToUnityScaled ()));
		if (lastRotatePinchSpot != Vector3.zero) {

			float angleAroundZ = Vector3.Angle (new Vector3 (lastRotatePinchSpot.x, lastRotatePinchSpot.y, 0), new Vector3 (pinchSpot.x, pinchSpot.y, 0));
			float angleAroundX = Vector3.Angle (new Vector3 (0, lastRotatePinchSpot.y, lastRotatePinchSpot.z), new Vector3 (0, pinchSpot.y, pinchSpot.z));
			float angleAroundY = Vector3.Angle (new Vector3 (lastRotatePinchSpot.x, 0, lastRotatePinchSpot.z), new Vector3 (pinchSpot.x, 0, pinchSpot.z));

			//vector.Angle gives acute angle- use sign to determine if change is necesary
			float signZ = Mathf.Sign (Vector3.Dot (new Vector3 (0, 0, 1), Vector3.Cross (new Vector3 (lastRotatePinchSpot.x, lastRotatePinchSpot.y, 0), new Vector3 (pinchSpot.x, pinchSpot.y, 0))));
			float signX = Mathf.Sign (Vector3.Dot (new Vector3 (1, 0, 0), Vector3.Cross (new Vector3 (0, lastRotatePinchSpot.y, lastRotatePinchSpot.z), new Vector3 (0, pinchSpot.y, pinchSpot.z))));
			float signY = Mathf.Sign (Vector3.Dot (new Vector3 (0, 1, 0), Vector3.Cross (new Vector3 (lastRotatePinchSpot.x, 0, lastRotatePinchSpot.z), new Vector3 (pinchSpot.x, 0, pinchSpot.z))));

			if (signZ < 0) {
					angleAroundZ = 360 - angleAroundZ;
			}
			if (signX < 0) {
					angleAroundX = 360 - angleAroundX;
			}
			if (signY < 0) {
					angleAroundY = 360 - angleAroundY;
			}
		
			if (objectPoint != level.transform.position) {
				
				drawnShape.transform.RotateAround (objectPoint, new Vector3 (0, 0, 1), angleAroundZ);
				drawnShape.transform.RotateAround (objectPoint, new Vector3 (1, 0, 0), angleAroundX);
				drawnShape.transform.RotateAround (objectPoint, new Vector3 (0, 1, 0), angleAroundY);

				lastAction =LastAction.ROTATE;
			}
			else
			{
				level.transform.RotateAround(objectPoint, Vector3.up, angleAroundY);
			}

		}else{
			lastRotation = drawnShape.transform.rotation;
			
		}
		lastRotatePinchSpot = pinchSpot;



	}

	void DrawOnIndexPoint(){
		//check just the index is pointing
			//draw
			
			Finger index = rightHand.Fingers [1];

		if (drawBox.collider.bounds.Contains (handController.transform.TransformPoint (index.JointPosition (Finger.FingerJoint.JOINT_TIP).ToUnityScaled ()))) {
			

			if (drawnShape == null) {
				NewShape (handController.transform.TransformPoint (index.TipPosition.ToUnityScaled ()), drawnShape);
			}
			if (!isDrawing) {
				isDrawing = true;
				//if(drawnShape != null){
				//	CopyShape();
				//}
			}
			lastAction = LastAction.DRAW;
			

			Points.Add (drawnShape.transform.InverseTransformPoint (handController.transform.TransformPoint (index.JointPosition (Finger.FingerJoint.JOINT_TIP).ToUnityScaled ())));
			Points.Add (drawnShape.transform.InverseTransformPoint (handController.transform.TransformPoint (index.JointPosition (Finger.FingerJoint.JOINT_PIP).ToUnityScaled ())));
			Points.Add (drawnShape.transform.InverseTransformPoint (handController.transform.TransformPoint (index.JointPosition (Finger.FingerJoint.JOINT_MCP).ToUnityScaled ())));

			UpdateMesh ();


		}

		
	}



	private void CreateMesh(){

		drawnShape.AddComponent ("MeshFilter");
		drawnShape.AddComponent ("MeshRenderer");
		drawnShape.AddComponent ("MeshCollider");
		mat = Resources.Load ("Materials/Shape") as Material;
		if(mat == null){
			Debug.LogError ("Materials not found.");
			return;
		}

		MeshFilter meshFilter = drawnShape.GetComponent<MeshFilter> ();
		if (meshFilter == null) {
			Debug.LogError("MeshFilter Not found.");
			return;
		}

		mesh = meshFilter.sharedMesh;
		if (mesh == null) {
			meshFilter.mesh = new Mesh();
			mesh = meshFilter.sharedMesh;
		}

		MeshCollider meshCollider = drawnShape.GetComponent<MeshCollider> ();
		if (meshCollider == null) {
			Debug.LogError("MeshCollider not found.");
			return;
		}
	    
		drawnShape.AddComponent("Rigidbody");
		drawnShape.rigidbody.useGravity = false;

	}

	public void UndoAction(){
		print ("undo");
		switch(lastAction){
			case LastAction.NONE:
				break;
			case LastAction.DRAW:
				drawnShape = lastObject;
				//Destroy(lastObject);
				NewShape(Vector3.zero, lastObject);
				drawnShape.SetActive(true);
			lastAction = LastAction.NONE;
				break;
			case LastAction.ROTATE:
				drawnShape.transform.rotation = lastRotation;
			lastAction = LastAction.NONE;
				break;
			case LastAction.TRANSLATE:
				drawnShape.transform.position = lastPosition;
			lastAction = LastAction.NONE;
				break;
		}
	}

	private void UpdateMesh(){

		Verts.Add (Points [Points.Count - 1]); Verts.Add (Points[Points.Count-2]); Verts.Add (Points [Points.Count - 3]);
		if(Verts.Count>3){

			//triangulate mesh - each triangle once per side
			Tris.Add(Verts.Count - 1); Tris.Add (Verts.Count-2); Tris.Add (Verts.Count-4);
			Tris.Add(Verts.Count - 4); Tris.Add (Verts.Count-2); Tris.Add (Verts.Count-1);


			Tris.Add(Verts.Count - 2); Tris.Add (Verts.Count-4); Tris.Add (Verts.Count-5);
			Tris.Add(Verts.Count - 5); Tris.Add (Verts.Count-4); Tris.Add (Verts.Count-2);

			Tris.Add(Verts.Count - 3); Tris.Add (Verts.Count-2); Tris.Add (Verts.Count-6);
			Tris.Add(Verts.Count - 6); Tris.Add (Verts.Count-2); Tris.Add (Verts.Count-3);

			
			Tris.Add(Verts.Count - 2); Tris.Add (Verts.Count-5); Tris.Add (Verts.Count-6);
			Tris.Add(Verts.Count - 6); Tris.Add (Verts.Count-5); Tris.Add (Verts.Count-2);


			mesh.vertices = Verts.ToArray();
			mesh.triangles = Tris.ToArray();
			mesh.RecalculateNormals ();
			mesh.RecalculateBounds ();
			MeshCollider meshCollider = drawnShape.GetComponent<MeshCollider>();

			meshCollider.sharedMesh = null;
			meshCollider.convex = true;
			meshCollider.sharedMesh = mesh;

			drawnShape.renderer.material = mat;
			mesh.Optimize();


		}

	}


}
