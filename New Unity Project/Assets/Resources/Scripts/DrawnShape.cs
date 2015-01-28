using UnityEngine;
using System.Collections;

public class DrawnShape : MonoBehaviour {

	ShapeState state; 

	enum ShapeState {draw, rotate, move, play};


	// Use this for initialization
	void Start () {
		//this.rigidbody.collider.
		//state = ShapeState.draw;
	}
	
	// Update is called once per frame
	void Update () {
//		if(this.rigidbody !=null){
////
////			if(state == ShapeState.draw){
////				this.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
////			}else if(state == ShapeState.rotate){
////				this.rigidbody.constraints = RigidbodyConstraints.None | RigidbodyConstraints.FreezePosition;
////			}else if (state == ShapeState.move || state == ShapeState.play){
////				this.rigidbody.constraints = RigidbodyConstraints.None ;
////			}
//
//		}
	}
}
