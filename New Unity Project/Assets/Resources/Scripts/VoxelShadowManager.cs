using UnityEngine;
using System.Collections;

public class VoxelShadowManager : MonoBehaviour {
	private static VoxelShadowManager _instance = null;

	protected VoxelShadowManager() {}

	public GameObject origin;
	public GameObject Shadow;
	public GameObject AllShadows;
	public GameObject floor;

	private Vector3 localGridOrigin;
	float SCALE = 0.2f;
	GameObject[,] shadowList = new GameObject[10,10];

	// Singleton pattern implementation
	public static VoxelShadowManager Instance { 
		get {
			if (_instance == null) {
				_instance = FindObjectOfType<VoxelShadowManager>();
				GameManager.Instance.OnStateChange += HandleOnStateChange;
				_instance.AllShadows.transform.parent = GameObject.FindGameObjectWithTag("Moving").transform;
				_instance.localGridOrigin = _instance.floor.transform.FindChild ("origin").transform.localPosition;
			}  
			return _instance;
		} 
	}

	static void HandleOnStateChange ()
	{
		if(GameManager.Instance.gameState == GameState.Drawing){
			foreach (GameObject go in _instance.shadowList){
				Destroy(go);
			}
			_instance.shadowList = new GameObject[10,10];
		}
	}
	
	public void ChangeShadowState(Vector2 shadowPosition, bool create) {

		if (shadowList [(int)shadowPosition.x, (int)shadowPosition.y] == null && create) {
			Vector3 worldPosition = floor.transform.TransformPoint(localGridOrigin + new Vector3 (shadowPosition.x * 0.1f,0.001f,-shadowPosition.y * 0.1f));

			shadowList[(int)shadowPosition.x,(int)shadowPosition.y] = (GameObject)Instantiate (Shadow, worldPosition,Quaternion.identity);
			shadowList[(int)shadowPosition.x,(int)shadowPosition.y].transform.parent = AllShadows.transform;
			shadowList[(int)shadowPosition.x,(int)shadowPosition.y].transform.localRotation = Shadow.transform.rotation;

		}
		if (shadowList [(int)shadowPosition.x, (int)shadowPosition.y] != null && !create) {
				Destroy (shadowList[(int)shadowPosition.x,(int)shadowPosition.y]);
		}
	}
}
