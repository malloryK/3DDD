using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class DrawMesh : MonoBehaviour {
	Mesh mesh;
	Material mat;
	List<Vector3> Points;
	List<Vector3> Verts;
	List<Vector3> Edge1;
	List<Vector3> Edge2;
	List<int> Tris;

	Hand rightHand;
	HandModel rightRigid;
	Hand leftHand;
	HandModel leftRigid;

	public GameObject prefabShape;
	GameObject drawnShape; 

	Vector3 lastPinchSpot;


	//float ANGULAR_DRAG = 10; //drag when spinning shape
	float CONFIDENCE_TO_DRAW = 0.1f; // confidence needed that hand is pointing to add to mesh
	float CONFIDENCE_TO_GRAB = 0.3f;
	public HandController handController;
		

	void Start () {

		Points = new List<Vector3>();
		Verts = new List<Vector3>();
		Tris = new List<int> ();

		//ClearShape ();
	}

	void NewShape(Vector3 position){

		Points.Clear ();
		Verts.Clear ();
		Tris.Clear ();

		drawnShape =  Instantiate (prefabShape,position , Quaternion.identity ) as GameObject;

		CreateMesh ();
		
	}


	void Update () {
		//Start new shape
		if (Input.GetKeyDown (KeyCode.N)) {
			drawnShape = null;
		}

		//get hands 
		rightHand = handController.GetFrame().Hands.Rightmost;	
		//rightRigid = handController.GetAllPhysicsHands ()[0];
		leftHand = handController.GetFrame ().Hands.Leftmost;
		if (!rightHand.IsRight) {
			rightHand = null;
		} 
		if (!leftHand.IsLeft) {
				leftHand = null;
		} 

		// if there is a right hand and we're confident of its state
		if(rightHand != null && rightHand.Confidence > CONFIDENCE_TO_DRAW){
			//check just the index is pointing
			if ( rightHand.Fingers[0].IsExtended == false && rightHand.Fingers[1].IsExtended == true &&  rightHand.Fingers[2].IsExtended == false && rightHand.Fingers[3].IsExtended == false && rightHand.Fingers[4].IsExtended == false)
			{
				DrawOnIndexPoint();
			}else if(rightHand.GrabStrength > CONFIDENCE_TO_GRAB){
				RotateDrawnShape();
			}else {
				lastPinchSpot = Vector3.zero;
			}
		}else {
			lastPinchSpot = Vector3.zero;
		}
		
	}

	void RotateDrawnShape ()
	{	

		Vector3 pinchSpot = drawnShape.transform.position-(handController.transform.TransformPoint(rightHand.Fingers[1].TipPosition.ToUnityScaled ()));
		if(lastPinchSpot != Vector3.zero){
		
			float angleAroundZ = Vector3.Angle (new Vector3(lastPinchSpot.x,lastPinchSpot.y,0), new Vector3(pinchSpot.x, pinchSpot.y, 0));
			float angleAroundX = Vector3.Angle (new Vector3(0,lastPinchSpot.y,lastPinchSpot.z), new Vector3(0,pinchSpot.y, pinchSpot.z));
			float angleAroundY = Vector3.Angle (new Vector3(lastPinchSpot.x,0,lastPinchSpot.z), new Vector3(pinchSpot.x,0,pinchSpot.z));

			//vector.Angle gives acute angle- use sign to determine if change is necesary
			float signZ = Mathf.Sign(Vector3.Dot(new Vector3(0,0,1),Vector3.Cross(new Vector3(lastPinchSpot.x,lastPinchSpot.y,0), new Vector3(pinchSpot.x, pinchSpot.y, 0))));
			float signX = Mathf.Sign(Vector3.Dot(new Vector3(1,0,0),Vector3.Cross(new Vector3(0,lastPinchSpot.y,lastPinchSpot.z), new Vector3(0,pinchSpot.y, pinchSpot.z))));
			float signY = Mathf.Sign(Vector3.Dot(new Vector3(0,1,0),Vector3.Cross(new Vector3(lastPinchSpot.x,0,lastPinchSpot.z), new Vector3(pinchSpot.x,0,pinchSpot.z))));

			if(signZ < 0){
				angleAroundZ = 360-angleAroundZ;
			}
			if(signX < 0){
				angleAroundX = 360-angleAroundX;
			}
			if(signY < 0){
				angleAroundY = 360-angleAroundY;
			}

	
			drawnShape.transform.RotateAround (drawnShape.transform.position, new Vector3(0,0,1), angleAroundZ );
			drawnShape.transform.RotateAround (drawnShape.transform.position, new Vector3(1,0,0), angleAroundX );
			drawnShape.transform.RotateAround (drawnShape.transform.position, new Vector3(0,1,0), angleAroundY );
		}
		lastPinchSpot = pinchSpot;



	}

	void DrawOnIndexPoint(){
		//check just the index is pointing
			

			//draw
			Finger index = rightHand.Fingers[1];

			if(drawnShape == null){
				NewShape(handController.transform.TransformPoint(index.TipPosition.ToUnityScaled()));
			}
			
			Points.Add (drawnShape.transform.InverseTransformPoint(handController.transform.TransformPoint(index.JointPosition(Finger.FingerJoint.JOINT_TIP).ToUnityScaled())));
			Points.Add (drawnShape.transform.InverseTransformPoint(handController.transform.TransformPoint(index.JointPosition(Finger.FingerJoint.JOINT_PIP).ToUnityScaled())));
			Points.Add (drawnShape.transform.InverseTransformPoint(handController.transform.TransformPoint(index.JointPosition(Finger.FingerJoint.JOINT_MCP).ToUnityScaled())));
			Edge1.Add (Points [Points.length - 3]);
			Edge2.Add (Points [Points.length-1]);

			UpdateMesh();

		
	}



	private void CreateMesh(){

		drawnShape.AddComponent ("MeshFilter");
		drawnShape.AddComponent ("MeshRenderer");
		drawnShape.AddComponent ("MeshCollider");
		mat = Resources.Load ("Materials/Default") as Material;
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
		meshCollider.convex = true;
		drawnShape.AddComponent("Rigidbody");
		drawnShape.rigidbody.useGravity = false;
		//drawnShape.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		//drawnShape.rigidbody.angularDrag = ANGULAR_DRAG; 
		drawnShape.collider.isTrigger = true;

	}

	IEnumerator FillMesh(){
		//connect ends
		Tris.Add (Edge1[0]);Tris.Add (Edge2[0]);Tris.Add (Edge2[Edge2.Count]);
		Tris.Add (Edge2[Edge2.Count]); Tris.Add (Edge1[Edge1.Count]);Tris.Add (Edge1[0]);

		//add triangles for edge1
		for(int i = 0; i<Edge1.Count-4; i++){
			Tris.Add(Edge1[i]); Tris.Add(Edge1[i+2]); Tris.Add(Edge1[i+4]);
		}
		Tris.Add (Edge1[Edge1.Count-3]); Tris.Add (Edge1[Edge1.Count-1]);Tris.Add (Edge1[1]);
		Tris.Add (Edge1[Edge1.Count-2]); Tris.Add (Edge1[0]);Tris.Add (Edge1[2]);

		//add triangles for edge2
		for(int i = 0; i<Edge2.Count-4; i++){
			Tris.Add(Edge2[i]); Tris.Add(Edge2[i+2]); Tris.Add(Edge2[i+4]);
		}
		Tris.Add (Edge2[Edge2.Count-3]); Tris.Add (Edge2[Edge2.Count-1]);Tris.Add (Edge2[1]);
		Tris.Add (Edge2[Edge2.Count-2]); Tris.Add (Edge2[0]);Tris.Add (Edge2[2]);



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

			MeshCollider meshCollider = drawnShape.GetComponent<MeshCollider>();
			mesh.RecalculateNormals ();
			mesh.RecalculateBounds ();
			meshCollider.sharedMesh = null;
			meshCollider.sharedMesh = mesh;
			drawnShape.renderer.material = mat;
			mesh.Optimize();


		}

	}


}
