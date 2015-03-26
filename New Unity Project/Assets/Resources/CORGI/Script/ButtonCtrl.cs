using UnityEngine;
using System.Collections;

public class ButtonCtrl : MonoBehaviour {


	public GameObject head;
	public GameObject glass_a;
	public GameObject glass_b;
	public GameObject glass_c;
	public GameObject pin;
	public GameObject neck;


	public GameObject frog;
	

	
	private Rect FpsRect ;
	private string frpString;
	
	private GameObject instanceObj;
	public GameObject[] gameObjArray=new GameObject[9];
	public AnimationClip[] AniList  = new AnimationClip[9];
	
	float minimum = 2.0f;
	float maximum = 50.0f;
	float touchNum = 0f;
	string touchDirection ="forward"; 


	void Start () {

		head.SetActive(false);
		glass_a.SetActive(false);
		glass_b.SetActive(false);
		glass_c.SetActive(false);
		pin.SetActive(false);
		neck.SetActive(false);

	}
	



	void OnGUI() {
		if (GUI.Button(new Rect(20, 20, 70, 40),"Idle")){
			frog.animation.wrapMode= WrapMode.Loop;
			frog.animation.CrossFade("0_idle");
			
		}
		if (GUI.Button(new Rect(90, 20, 70, 40),"Walk")){
			frog.animation.wrapMode= WrapMode.Loop;
			frog.animation.CrossFade("1_Walk");
			
		}
		if (GUI.Button(new Rect(160, 20, 70, 40),"Run")){
			frog.animation.wrapMode= WrapMode.Loop;
			frog.animation.CrossFade("2_Run");
		}
		if (GUI.Button(new Rect(230, 20, 70, 40),"Jump")){
			frog.animation.wrapMode= WrapMode.Loop;
			frog.animation.CrossFade("3_Jump");
		}
		if (GUI.Button(new Rect(300, 20, 70, 40),"Attack")){
			frog.animation.wrapMode= WrapMode.Loop;
			frog.animation.CrossFade("4_Attack");
		}
		if (GUI.Button(new Rect(370, 20, 70, 40),"Pain")){
			frog.animation.wrapMode= WrapMode.Loop;
			frog.animation.CrossFade("5_Pain");
		} 
		if (GUI.Button(new Rect(440, 20, 70, 40),"Dead")){
			frog.animation.wrapMode= WrapMode.Loop;
			frog.animation.CrossFade("6_Dead");
		}  
		if (GUI.Button(new Rect(50, 260, 70, 40),"Basic")){
			head.SetActive(false);
			glass_a.SetActive(false);
			glass_b.SetActive(false);
			glass_c.SetActive(false);
			pin.SetActive(false);
			neck.SetActive(false);
		}
		if (GUI.Button(new Rect(50, 300, 70, 40),"Head")){
			head.SetActive(true);
			glass_a.SetActive(false);
			glass_b.SetActive(false);
			glass_c.SetActive(false);
		
		}
		if (GUI.Button(new Rect(50, 340, 70, 40),"Glass_A")){
			head.SetActive(false);
			glass_a.SetActive(true);
			glass_b.SetActive(false);
			glass_c.SetActive(false);

		}
		if (GUI.Button(new Rect(50, 380, 70, 40),"Glass_B")){
			head.SetActive(false);
			glass_a.SetActive(false);
			glass_b.SetActive(true);
			glass_c.SetActive(false);
			
		}
		if (GUI.Button(new Rect(50, 420, 70, 40),"Glass_C")){
			head.SetActive(false);
			glass_a.SetActive(false);
			glass_b.SetActive(false);
			glass_c.SetActive(true);
			
		}
		if (GUI.Button(new Rect(50, 460, 70, 40),"Pin")){
			pin.SetActive(true);

			
		}
		if (GUI.Button(new Rect(50, 500, 70, 40),"Neck")){
			neck.SetActive(true);
			
			
		}



	}
	

	// Update is called once per frame
	void Quit () {
		Application.Quit();

	}
}

