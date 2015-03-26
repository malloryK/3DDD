using UnityEngine;
using System.Collections;

public class VoxelShadowManager : MonoBehaviour {
	private static VoxelShadowManager _instance = null;

	protected VoxelShadowManager() {}

	public GameObject origin;
	public GameObject Shadow;
	public GameObject AllShadows;
	float SCALE = 0.2f;
	GameObject[,] shadowList = new GameObject[10,10];

	// Singleton pattern implementation
	public static VoxelShadowManager Instance { 
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<VoxelShadowManager>();
			}  
			return _instance;
		} 
	}
	
	public void ChangeShadowState(Vector2 shadowPosition, bool create) {

		if (shadowList [(int)shadowPosition.x, (int)shadowPosition.y] == null && create) {
			shadowList[(int)shadowPosition.x,(int)shadowPosition.y] = (GameObject)Instantiate (Shadow, origin.transform.position+(new Vector3(shadowPosition.x,0.001f,  -shadowPosition.y )*SCALE), Shadow.transform.rotation);
			shadowList[(int)shadowPosition.x,(int)shadowPosition.y].transform.parent = AllShadows.transform;
		}
		if (shadowList [(int)shadowPosition.x, (int)shadowPosition.y] != null && !create) {
				Destroy (shadowList[(int)shadowPosition.x,(int)shadowPosition.y]);
		}
	}
}
