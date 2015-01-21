using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class OnClickMesh : MonoBehaviour {
	Mesh mesh;
	Material mat;
	List<Vector3> Points;
	List<Vector3> Verts;
	List<int> Tris;
	//List<Vector2> UVs;
	Hand rightHand;
	Hand leftHand;

	GameObject meshObject;


	public HandController handController;
		

	void Start () {
		meshObject = Instantiate (new GameObject(),new Vector3 (0.5f, 0, 0) , Quaternion.identity ) as GameObject;//new GameObject();
	
		Points = new List<Vector3>();
		Verts = new List<Vector3>();
		Tris = new List<int> ();
	//	UVs = new List<Vector2>();
		CreateMesh ();
	
		
		
		
	}
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.R)) {
			meshObject.AddComponent<Rigidbody>();
		}


		rightHand = handController.GetFrame().Hands.Rightmost;
		leftHand = handController.GetFrame ().Hands.Leftmost;
		Vector3 drawPosition = handController.transform.TransformPoint(rightHand.PalmPosition.ToUnityScaled());

	
		if (Input.GetKey(KeyCode.Space) ) {


			//draw
			Finger pinky = rightHand.Fingers[4];
			Points.Add (meshObject.transform.InverseTransformPoint(handController.transform.TransformPoint(pinky.JointPosition(Finger.FingerJoint.JOINT_DIP).ToUnityScaled())));
			Points.Add (meshObject.transform.InverseTransformPoint(handController.transform.TransformPoint(pinky.JointPosition(Finger.FingerJoint.JOINT_PIP).ToUnityScaled())));
			Points.Add (meshObject.transform.InverseTransformPoint(handController.transform.TransformPoint(pinky.JointPosition(Finger.FingerJoint.JOINT_MCP).ToUnityScaled())));
			UpdateMesh();




		}
	}

	private void CreateMesh(){

		meshObject.AddComponent ("MeshFilter");
		meshObject.AddComponent ("MeshRenderer");
		meshObject.AddComponent ("MeshCollider");
		mat = Resources.Load ("Materials/Default") as Material;
		if(mat == null){
			Debug.LogError ("Materials not found.");
			return;
		}

		MeshFilter meshFilter = meshObject.GetComponent<MeshFilter> ();
		if (meshFilter == null) {
			Debug.LogError("MeshFilter Not found.");
			return;
		}

		mesh = meshFilter.sharedMesh;
		if (mesh == null) {
			meshFilter.mesh = new Mesh();
			mesh = meshFilter.sharedMesh;
		}

		MeshCollider meshCollider = meshObject.GetComponent<MeshCollider> ();
		if (meshCollider == null) {
			Debug.LogError("MeshCollider not found.");
			return;
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

			MeshCollider meshCollider = meshObject.GetComponent<MeshCollider>();
			mesh.RecalculateNormals ();
			mesh.RecalculateBounds ();
			meshCollider.sharedMesh = null;
			meshCollider.sharedMesh = mesh;
			meshObject.renderer.material = mat;
			mesh.Optimize();


		}

	}


}
