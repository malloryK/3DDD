using UnityEngine;
using System.Collections;
using Leap;

public class LevelRotate : MonoBehaviour {
	public HandController handController;
	Hand rightHand;
	Hand leftHand;
	float CONFIDENCE_TO_GRAB = 0.8f;
	float MIN_HAND_CONFIDENCE = 0.1f;
	Vector3 lastRotatePinchSpot;
	GameManager GM;

	// Use this for initialization
	void Start () {
		GM = GameManager.Instance;

	}
	
	// Update is called once per frame
	void Update () {
		SetHands ();
		if (rightHand != null && IsGrabbing (rightHand)) {
			rotateLevel ();
		} else {
			lastRotatePinchSpot = Vector3.zero;
		}
	}
	void rotateLevel()
	{

//		Vector3 origin = drawingBox.transform.FindChild ("origin").transform.localPosition;
		if (GM.gameState == GameState.Drawing) {
			Vector3 pinchSpot = this.transform.position - (SkeletalHand.RealTip.position);
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
				this.transform.RotateAround (this.transform.position, Vector3.up, angleAroundY);
			}
			lastRotatePinchSpot = pinchSpot;
		}
	}
	bool IsGrabbing(Hand hand){
		if (hand.Confidence > MIN_HAND_CONFIDENCE) {
			return rightHand.GrabStrength > CONFIDENCE_TO_GRAB;
		}
		else {
			return false;
		}
	}

	void SetHands(){
		
		//get hands 
		float lowest = handController.GetFrame().Hands[0].Confidence;
		int delete = 0;
		while (handController.GetFrame().Hands.Count > 2) {
			for(int i = 0; i< handController.GetFrame().Hands.Count; i++)
			{
				if(handController.GetFrame().Hands[i].Confidence < lowest)
				{
					delete = i;
				}
			}
			handController.GetFrame().Hands[delete].Dispose();
		}
		
		rightHand = handController.GetFrame().Hands.Rightmost;    
		leftHand = handController.GetFrame().Hands.Leftmost;
		
		//check for correct handedness
		Hand temp = null;
		if (!rightHand.IsRight) {
			temp = rightHand;
			rightHand = null;
			
		} else if (rightHand.Confidence < MIN_HAND_CONFIDENCE) {
			rightHand = null;
		}
		if (!leftHand.IsLeft) {
			rightHand = leftHand;
			if (temp != null) {
				leftHand = temp;
			} else {
				leftHand = null;
			}
		} else if (leftHand.Confidence < MIN_HAND_CONFIDENCE) {
			leftHand = null;
		}
	}
}
