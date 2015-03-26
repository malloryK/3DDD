using UnityEngine;
using System.Collections;

public class WinLoseCheck : MonoBehaviour {
	GameManager GM;

	// Use this for initialization
	void Start () {
		GM = GameManager.Instance;
		GM.OnStateChange+= HandleOnStateChange;
	}
	
	// Update is called once per frame
	void HandleOnStateChange () {
	
	}
}
