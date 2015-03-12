using UnityEngine;
using System.Collections;
using System;
using Leap;

public class VoxelManager : MonoBehaviour {

	public HandController handController;
	public GameObject drawingBox;
	public GameObject voxel;

	int DRAW_WIDTH = 10;
	int DRAW_LENGTH = 10;
	int DRAW_HEIGHT = 5;
	float MIN_HAND_CONFIDENCE = 0.1f;
	Vector3 gridOrigin;

	GameObject[,,] voxelGrid;
	GameObject createdObject;
	Hand rightHand;
	Hand leftHand;

	// Use this for initialization
	void Start () {
		createdObject = new GameObject ();
		voxelGrid = new GameObject[DRAW_WIDTH,DRAW_LENGTH,DRAW_HEIGHT];
		gridOrigin = drawingBox.transform.FindChild ("origin").transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		SetHands ();
		Vector3 gridPosition;
		if (rightHand != null && IsPointing (rightHand)) {
			CreateVoxel( IndexFingerTip (rightHand));
		}
	}

	//creates voxel in the grid where the tip is pointing
	void CreateVoxel(Vector3 fingerTip){
		Vector3 gridPosition = (fingerTip - gridOrigin)*5;
		print (fingerTip);
		print (gridOrigin);
		print (gridPosition);
		//ensure hand is within the bounding box
		if (gridPosition != null && gridPosition.x <= DRAW_WIDTH && gridPosition.z <= DRAW_LENGTH && gridPosition.y <= DRAW_HEIGHT) {
			Vector3 roundedGridPosition = new Vector3 ((int)Math.Round(gridPosition.x), (int)Math.Round(gridPosition.y), (int)Math.Round(gridPosition.z));
			if(voxelGrid[(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z]==null){
				Vector3 worldPosition = gridOrigin + new Vector3 (roundedGridPosition.x * 0.2f, roundedGridPosition.y * 0.2f, roundedGridPosition.z * 0.2f);
				voxelGrid [(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z] = (GameObject)Instantiate (voxel, worldPosition, voxel.transform.rotation);
				voxelGrid [(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z].transform.parent = createdObject.transform;
			}
		}
	}


	//checks if hands index finger is the only one pointing
	bool IsPointing(Hand hand){
		if (hand.Confidence > MIN_HAND_CONFIDENCE) {
			return hand.Fingers [0].IsExtended == false && rightHand.Fingers [1].IsExtended == true && 
				rightHand.Fingers [2].IsExtended == false && rightHand.Fingers [3].IsExtended == false && rightHand.Fingers [4].IsExtended == false;
		} else {
			return false;
		}
	}

	//Returns the hand index finger tip locatin in world coords
	Vector3 IndexFingerTip(Hand hand){
		Finger index = hand.Fingers [1];
		return handController.transform.TransformPoint (index.TipPosition.ToUnityScaled ());
	}

	void SetHands(){
		//get hands 
		rightHand = handController.GetFrame().Hands.Rightmost;	
		leftHand = handController.GetFrame().Hands.Leftmost;
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
	}
}
