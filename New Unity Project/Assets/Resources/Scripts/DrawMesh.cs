using UnityEngine;
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

	public GameObject prefabShape;
	GameObject DrawnShape; 


	public HandController handController;
		

	void Start () {

		Points = new List<Vector3>();
		Verts = new List<Vector3>();
		Tris = new List<int> ();

		ClearShape ();
	
		
		
		
	}

	void ClearShape(){

		Points.Clear ();
		Verts.Clear ();
		Tris.Clear ();

		DrawnShape =  Instantiate (prefabShape,new Vector3 (0.5f, 0, 0) , Quaternion.identity ) as GameObject;

		CreateMesh ();
		
	}


	void Update () {
		//Start new shape
		if (Input.GetKeyDown (KeyCode.N)) {
			ClearShape();
		}

		//get hands 
		rightHand = handController.GetFrame().Hands.Rightmost;	
		leftHand = handController.GetFrame ().Hands.Leftmost;
		if (!rightHand.IsRight) {
			rightHand = null;
		} 
		if (!leftHand.IsLeft) {
				leftHand = null;
		} 

		//draw if there is a right hand
		if(rightHand != null){

			DrawOnIndexPoint();

		}
	}

	void DrawOnIndexPoint(){
		//check just the index is pointing
		if ( rightHand.Fingers[0].IsExtended == false && rightHand.Fingers[1].IsExtended == true &&  rightHand.Fingers[2].IsExtended == false && rightHand.Fingers[3].IsExtended == false && rightHand.Fingers[4].IsExtended == false)
		{
			//draw
			Vector3 drawPosition = handController.transform.TransformPoint(rightHand.PalmPosition.ToUnityScaled());
			Finger index = rightHand.Fingers[1];
			Points.Add (DrawnShape.transform.InverseTransformPoint(handController.transform.TransformPoint(index.JointPosition(Finger.FingerJoint.JOINT_TIP).ToUnityScaled())));
			Points.Add (DrawnShape.transform.InverseTransformPoint(handController.transform.TransformPoint(index.JointPosition(Finger.FingerJoint.JOINT_PIP).ToUnityScaled())));
			Points.Add (DrawnShape.transform.InverseTransformPoint(handController.transform.TransformPoint(index.JointPosition(Finger.FingerJoint.JOINT_MCP).ToUnityScaled())));
			UpdateMesh();
			
		}
	}



	private void CreateMesh(){

		DrawnShape.AddComponent ("MeshFilter");
		DrawnShape.AddComponent ("MeshRenderer");
		DrawnShape.AddComponent ("MeshCollider");
		mat = Resources.Load ("Materials/Default") as Material;
		if(mat == null){
			Debug.LogError ("Materials not found.");
			return;
		}

		MeshFilter meshFilter = DrawnShape.GetComponent<MeshFilter> ();
		if (meshFilter == null) {
			Debug.LogError("MeshFilter Not found.");
			return;
		}

		mesh = meshFilter.sharedMesh;
		if (mesh == null) {
			meshFilter.mesh = new Mesh();
			mesh = meshFilter.sharedMesh;
		}

		MeshCollider meshCollider = DrawnShape.GetComponent<MeshCollider> ();
		if (meshCollider == null) {
			Debug.LogError("MeshCollider not found.");
			return;
		}
		meshCollider.convex = true;
		DrawnShape.AddComponent("Rigidbody");
		DrawnShape.rigidbody.useGravity = false;

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

			MeshCollider meshCollider = DrawnShape.GetComponent<MeshCollider>();
			mesh.RecalculateNormals ();
			mesh.RecalculateBounds ();
			meshCollider.sharedMesh = null;
			meshCollider.sharedMesh = mesh;
			DrawnShape.renderer.material = mat;
			mesh.Optimize();


		}

	}


}
