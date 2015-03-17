using UnityEngine;
using System.Collections;
using System;
using Leap;

public class VoxelManager : MonoBehaviour {

	public HandController handController;
	public GameObject drawingBox;
	public GameObject voxel;
	public int numberOfVoxelsAllowed = 10;
	
	int DRAW_WIDTH = 10;
	int DRAW_LENGTH = 10;
	int DRAW_HEIGHT = 5;
	float MIN_HAND_CONFIDENCE = 0.1f;
	Vector3 localGridOrigin;
	Vector3 globalGridOrigin;

	int currentNumberOFVoxels = 0;
	GameObject[,,] voxelGrid;
	GameObject createdObject;
	Hand rightHand;
	Hand leftHand;

	// Use this for initialization
	void Start () {
		createdObject = new GameObject ();
		createdObject.AddComponent<Rigidbody>();
		createdObject.rigidbody.constraints = RigidbodyConstraints.FreezeAll ^ RigidbodyConstraints.FreezePositionY;
		voxelGrid = new GameObject[DRAW_WIDTH,DRAW_LENGTH,DRAW_HEIGHT];
		localGridOrigin = drawingBox.transform.FindChild ("origin").transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
		SetHands ();
		Vector3 gridPosition;
		//pointng with index then create
		if (rightHand != null && IsPointing (rightHand) && currentNumberOFVoxels<=numberOfVoxelsAllowed) {
			if(SkeletalHand.RealTip != null){
				CreateVoxel( SkeletalHand.RealTip.position);
			}
		//pointing with pinky then delete
		}else if (rightHand != null && IsPinky (rightHand) && currentNumberOFVoxels>0) {
			if(SkeletalHand.PinkyTip != null){
				DeleteVoxel( SkeletalHand.PinkyTip.position);
			}
		}

	}


	void DeleteVoxel(Vector3 fingerTip){

		Vector3 fingerPosInBox = drawingBox.transform.InverseTransformPoint (fingerTip);
		Vector3 gridPosition = (fingerPosInBox - localGridOrigin)*10;
		Vector3 roundedGridPosition = new Vector3 ((int)Math.Round(gridPosition.x), (int)Math.Round(gridPosition.y), (int)Math.Round(gridPosition.z));
		if (roundedGridPosition.x < DRAW_WIDTH && roundedGridPosition.z < DRAW_HEIGHT && roundedGridPosition.y < DRAW_LENGTH 
			&& roundedGridPosition.x > 0 && roundedGridPosition.z > 0 && roundedGridPosition.y > 0) {
			
			if (voxelGrid [(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z] != null) {
				Destroy(voxelGrid [(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z]);
				currentNumberOFVoxels--;
			}
		}


	}

	//creates voxel in the grid where the tip is pointing
	void CreateVoxel(Vector3 fingerTip){

		Vector3 fingerPosInBox = drawingBox.transform.InverseTransformPoint (fingerTip);
		Vector3 gridPosition = (fingerPosInBox - localGridOrigin)*10;
		Vector3 roundedGridPosition = new Vector3 ((int)Math.Round(gridPosition.x), (int)Math.Round(gridPosition.y), (int)Math.Round(gridPosition.z));

		if (roundedGridPosition.x < DRAW_WIDTH && roundedGridPosition.z < DRAW_HEIGHT && roundedGridPosition.y < DRAW_LENGTH 
		    && roundedGridPosition.x >0 && roundedGridPosition.z>0 && roundedGridPosition.y >0) {

			if(voxelGrid[(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z]==null){
				currentNumberOFVoxels++;
				Vector3 worldPosition = drawingBox.transform.TransformPoint(localGridOrigin + new Vector3 (roundedGridPosition.x * 0.1f,roundedGridPosition.y * 0.1f,roundedGridPosition.z * 0.1f));

				voxelGrid [(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z] = (GameObject)Instantiate (voxel, worldPosition, voxel.transform.rotation);
				voxelGrid [(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z].transform.parent = createdObject.transform;
			}
		}
	}




	bool IsPinky(Hand hand){
		if (hand.Confidence > MIN_HAND_CONFIDENCE) {
			return hand.Fingers [0].IsExtended == false && rightHand.Fingers [1].IsExtended == false && 
				rightHand.Fingers [2].IsExtended == false && rightHand.Fingers [3].IsExtended == false && rightHand.Fingers [4].IsExtended == true;
		} else {
			return false;
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
