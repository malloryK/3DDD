using UnityEngine;
using System.Collections;
using System;
using Leap;

public class VoxelManager : MonoBehaviour {
	
	
	public HandController handController;
	public GameObject drawingBox;
	public GameObject voxel;
	public int numberOfVoxelsAllowed = 10;
	
	VoxelShadowManager VM;
	GameManager GM;
	
	static int DRAW_WIDTH = 10;
	static int DRAW_LENGTH = 10;
	static int DRAW_HEIGHT = 5;
	float MIN_HAND_CONFIDENCE = 0.1f;
	Vector3 localGridOrigin;
	Vector3 globalGridOrigin;
	
	int currentNumberOFVoxels = 0;
	GameObject[,,] voxelGrid;
	GameObject createdObject;
	Hand rightHand;
	Hand leftHand;
	
	private bool [,,] visited = new bool[DRAW_WIDTH,DRAW_LENGTH,DRAW_HEIGHT];
	private int minX, maxX, minY, maxY, minZ, maxZ;
	private Vector3 minVector, maxVector;
	private Vector3[,] regionsMinMax = new Vector3[6,2];
	//private int[,] regionsMinMaxInts = new int[6,6];
	
	
	// Use this for initialization
	void Start () {
		VM = VoxelShadowManager.Instance;
		GM = GameManager.Instance;
		GM.OnStateChange+= HandleOnStateChange;
		GM.SetGameState (GameState.Drawing);
		createdObject = new GameObject ();
		createdObject.transform.parent = GameObject.FindGameObjectWithTag("Moving").transform;
		createdObject.AddComponent<Rigidbody>();
		createdObject.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		voxelGrid = new GameObject[DRAW_WIDTH,DRAW_LENGTH,DRAW_HEIGHT];
		localGridOrigin = drawingBox.transform.FindChild ("origin").transform.localPosition;
	}
	
	void HandleOnStateChange () {
		if (GM.gameState == GameState.Firing) {
			drawingBox.SetActive(false);
			createdObject.rigidbody.constraints = (RigidbodyConstraints.FreezeAll ^ RigidbodyConstraints.FreezePositionY) ^ RigidbodyConstraints.FreezeRotationY;
		}else if(GM.gameState == GameState.Drawing){
			drawingBox.SetActive(true);
			Destroy(createdObject);
			currentNumberOFVoxels = 0;
			createdObject = new GameObject ();
			createdObject.AddComponent<Rigidbody>();
			createdObject.rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			voxelGrid = new GameObject[DRAW_WIDTH,DRAW_LENGTH,DRAW_HEIGHT];
		}
	}
	
	
	// Update is called once per frame
	void Update () {
		SetHands ();
		if (GM.gameState == GameState.Drawing) {
			Vector3 gridPosition;
			//pointng with index then create
			if (rightHand != null && IsPointing (rightHand) && currentNumberOFVoxels <= numberOfVoxelsAllowed) {
				if (SkeletalHand.RealTip != null) {
					CreateVoxel (SkeletalHand.RealTip.position);
				}
				//pointing with pinky then delete
			} else if (rightHand != null && IsPinky (rightHand) && currentNumberOFVoxels > 0) {
				if (SkeletalHand.PinkyTip != null) {
					DeleteVoxel (SkeletalHand.PinkyTip.position);
				}
			}
		}
	}
	
	
	void DeleteVoxel(Vector3 fingerTip){
		
		Vector3 fingerPosInBox = drawingBox.transform.InverseTransformPoint (fingerTip);
		Vector3 gridPosition = (fingerPosInBox - localGridOrigin)*10;
		Vector3 roundedGridPosition = new Vector3 ((int)Math.Round(gridPosition.x), (int)Math.Round(gridPosition.y), (int)Math.Round(gridPosition.z));
		if (roundedGridPosition.x < DRAW_WIDTH && roundedGridPosition.z < DRAW_HEIGHT && roundedGridPosition.y < DRAW_LENGTH 
		    && roundedGridPosition.x > 0 && roundedGridPosition.z > 0 && roundedGridPosition.y > 0) {
			
			if (voxelGrid [(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z] != null  && canDeleteVoxel(roundedGridPosition)) {

				Destroy(voxelGrid [(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z]);
				voxelGrid[(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z] = null;
				
				bool emptyColumn = true;
				for (int i = 0; i < DRAW_HEIGHT; i ++){
					if (voxelGrid[(int)roundedGridPosition.x, (int)roundedGridPosition.y, i] != null){
						emptyColumn = false;
					}
				}
				
				if (emptyColumn){
					VM.ChangeShadowState(new Vector2((int)roundedGridPosition.x, (int) roundedGridPosition.y), false);
				}
				currentNumberOFVoxels--;
			}
		}
		
		
	}
	
	//creates voxel in the grid where the tip is pointing
	void CreateVoxel(Vector3 fingerTip){
		
		Vector3 fingerPosInBox = drawingBox.transform.InverseTransformPoint (fingerTip);
		Vector3 gridPosition = (fingerPosInBox - localGridOrigin)*10;
		Vector3 roundedGridPosition = new Vector3 ((int)Math.Round(gridPosition.x), (int)Math.Round(gridPosition.y), (int)Math.Round(gridPosition.z));
		
		//within bounds of draw box
		if (roundedGridPosition.x < DRAW_WIDTH && roundedGridPosition.z < DRAW_HEIGHT && roundedGridPosition.y < DRAW_LENGTH 
		    && roundedGridPosition.x >0 && roundedGridPosition.z>0 && roundedGridPosition.y >0) {
			
			//no current voxel exists
			if(voxelGrid[(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z]==null && canCreateVoxel (roundedGridPosition)){
				currentNumberOFVoxels++;
				Vector3 worldPosition = drawingBox.transform.TransformPoint(localGridOrigin + new Vector3 (roundedGridPosition.x * 0.1f,roundedGridPosition.y * 0.1f,roundedGridPosition.z * 0.1f));
				VM.ChangeShadowState(new Vector2((int)roundedGridPosition.x, (int) roundedGridPosition.y), true);
				voxelGrid [(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z] = (GameObject)Instantiate (voxel, worldPosition, Quaternion.identity);
				voxelGrid [(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z].transform.parent = createdObject.transform;
				voxelGrid [(int)roundedGridPosition.x, (int)roundedGridPosition.y, (int)roundedGridPosition.z].transform.localRotation = Quaternion.identity;
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
	
	bool canCreateVoxel(Vector3 gridCoordinates)
	{
		//first voxel can be placed anywhere
		if (currentNumberOFVoxels == 0)
			return true;
		
		//has at least one adjacent voxel, can create
		if (((int)gridCoordinates.x < DRAW_WIDTH - 1 && (voxelGrid [(int)gridCoordinates.x + 1, (int)gridCoordinates.y, (int)gridCoordinates.z] != null)) ||
		    ((int)gridCoordinates.x > 0 && (voxelGrid [(int)gridCoordinates.x - 1, (int)gridCoordinates.y, (int)gridCoordinates.z] != null)) ||
		    ((int)gridCoordinates.y < DRAW_LENGTH - 1 && (voxelGrid [(int)gridCoordinates.x, (int)gridCoordinates.y + 1, (int)gridCoordinates.z] != null)) ||
		    ((int)gridCoordinates.y > 0 && (voxelGrid [(int)gridCoordinates.x, (int)gridCoordinates.y - 1, (int)gridCoordinates.z] != null)) ||
		    ((int)gridCoordinates.z < DRAW_HEIGHT - 1 && (voxelGrid [(int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z + 1] != null)) ||
		    ((int)gridCoordinates.z > 0 && (voxelGrid [(int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z - 1] != null))) {
			
			return true;
		}
		
		return false;
	}
	
	bool canDeleteVoxel(Vector3 gridCoordinates)
	{
		//we can delete the last voxel
		if (currentNumberOFVoxels == 1)
			return true;
		
		//region grow from each neighbour to see if the remaining shape is still contiguous
		//each grown reagion produces a shape, we check if the shapes match.
		
		//if only one neighbour allow deletion
		int neighbours = 0;
		
		if ((int)gridCoordinates.x < DRAW_WIDTH - 1 && voxelGrid [(int)gridCoordinates.x + 1, (int)gridCoordinates.y, (int)gridCoordinates.z] != null) {
			neighbours++;
		}
		if ((int)gridCoordinates.x > 0 && voxelGrid [(int)gridCoordinates.x - 1, (int)gridCoordinates.y, (int)gridCoordinates.z] != null) {
			neighbours++;
		}
		if ((int)gridCoordinates.y < DRAW_LENGTH - 1 && voxelGrid [(int)gridCoordinates.x, (int)gridCoordinates.y + 1, (int)gridCoordinates.z] != null) {
			neighbours++;
		}
		if ((int)gridCoordinates.y > 0 && voxelGrid [(int)gridCoordinates.x, (int)gridCoordinates.y - 1, (int)gridCoordinates.z] != null) {
			neighbours++;
		}
		if ((int)gridCoordinates.z < DRAW_HEIGHT - 1 && voxelGrid [(int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z + 1] != null) {
			neighbours++;
		}
		if ((int)gridCoordinates.z > 0 && voxelGrid [(int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z - 1] != null) {
			neighbours++;
		}
		
		if (neighbours == 1)
			return true;
		
		
		Array.Clear (regionsMinMax, 0, regionsMinMax.Length);
		
		if ((int)gridCoordinates.x < DRAW_WIDTH -1 && voxelGrid [(int)gridCoordinates.x + 1, (int)gridCoordinates.y, (int)gridCoordinates.z] != null) {
			
			//clear array and reset min maxes on this voxel
			Array.Clear(visited, 0, visited.Length);
			minX = (int)gridCoordinates.x + 1;
			maxX = (int)gridCoordinates.x + 1;
			minY = (int)gridCoordinates.y;
			maxY = (int)gridCoordinates.y;
			minZ = (int)gridCoordinates.z;
			maxZ = (int)gridCoordinates.z;
			
			//deletion candidate set to visited, so cannot grow shape through this voxel.
			visited[(int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z] = true;
			
			//grow region from the neighbour.
			growRegion ((int)gridCoordinates.x + 1, (int)gridCoordinates.y, (int)gridCoordinates.z);
			
			//after growing, grab min, max values for comparison later.
			minVector = new Vector3(minX, minY, minZ);
			maxVector = new Vector3(maxX, maxY, maxZ);
			
			regionsMinMax[0,0] = minVector;
			regionsMinMax[0,1] = maxVector;
			
		}
		
		if ((int)gridCoordinates.x > 0 && voxelGrid [(int)gridCoordinates.x - 1, (int)gridCoordinates.y, (int)gridCoordinates.z] != null) {
			
			//clear array and reset min maxes on this voxel
			Array.Clear(visited, 0, visited.Length);
			minX = (int)gridCoordinates.x - 1;
			maxX = (int)gridCoordinates.x - 1;
			minY = (int)gridCoordinates.y;
			maxY = (int)gridCoordinates.y;
			minZ = (int)gridCoordinates.z;
			maxZ = (int)gridCoordinates.z;
			
			//deletion candidate set to visited, so cannot grow shape through this voxel.
			visited[(int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z] = true;
			
			//grow region from the neighbour.
			growRegion ((int)gridCoordinates.x - 1, (int)gridCoordinates.y, (int)gridCoordinates.z);
			
			//after growing, grab min, max values for comparison later.
			minVector = new Vector3(minX, minY, minZ);
			maxVector = new Vector3(maxX, maxY, maxZ);
			
			regionsMinMax[1,0] = minVector;
			regionsMinMax[1,1] = maxVector;
		}
		
		if ((int)gridCoordinates.y < DRAW_LENGTH-1 && voxelGrid [(int)gridCoordinates.x, (int)gridCoordinates.y + 1, (int)gridCoordinates.z] != null) {
			
			//clear array and reset min maxes on this voxel
			Array.Clear(visited, 0, visited.Length);
			minX = (int)gridCoordinates.x;
			maxX = (int)gridCoordinates.x;
			minY = (int)gridCoordinates.y + 1;
			maxY = (int)gridCoordinates.y + 1;
			minZ = (int)gridCoordinates.z;
			maxZ = (int)gridCoordinates.z;
			
			//deletion candidate set to visited, so cannot grow shape through this voxel.
			visited[(int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z] = true;
			
			//grow region from the neighbour.
			growRegion ((int)gridCoordinates.x, (int)gridCoordinates.y + 1, (int)gridCoordinates.z);
			
			//after growing, grab min, max values for comparison later.
			minVector = new Vector3(minX, minY, minZ);
			maxVector = new Vector3(maxX, maxY, maxZ);
			
			regionsMinMax[2,0] = minVector;
			regionsMinMax[2,1] = maxVector;
			
		}
		
		if ((int)gridCoordinates.y > 0 && voxelGrid [(int)gridCoordinates.x, (int)gridCoordinates.y - 1, (int)gridCoordinates.z] != null) {
			//clear array and reset min maxes on this voxel
			Array.Clear(visited, 0, visited.Length);
			minX = (int)gridCoordinates.x;
			maxX = (int)gridCoordinates.x;
			minY = (int)gridCoordinates.y - 1;
			maxY = (int)gridCoordinates.y - 1;
			minZ = (int)gridCoordinates.z;
			maxZ = (int)gridCoordinates.z;
			
			//deletion candidate set to visited, so cannot grow shape through this voxel.
			visited[(int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z] = true;
			
			//grow region from the neighbour.
			growRegion ((int)gridCoordinates.x, (int)gridCoordinates.y - 1, (int)gridCoordinates.z);
			
			//after growing, grab min, max values for comparison later.
			minVector = new Vector3(minX, minY, minZ);
			maxVector = new Vector3(maxX, maxY, maxZ);
			
			regionsMinMax[3,0] = minVector;
			regionsMinMax[3,1] = maxVector;
		}
		
		if ((int)gridCoordinates.z < DRAW_HEIGHT-1 && voxelGrid [(int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z + 1] != null) {
			//clear array and reset min maxes on this voxel
			Array.Clear(visited, 0, visited.Length);
			minX = (int)gridCoordinates.x;
			maxX = (int)gridCoordinates.x;
			minY = (int)gridCoordinates.y;
			maxY = (int)gridCoordinates.y;
			minZ = (int)gridCoordinates.z + 1;
			maxZ = (int)gridCoordinates.z + 1;
			
			//deletion candidate set to visited, so cannot grow shape through this voxel.
			visited[(int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z] = true;
			
			//grow region from the neighbour.
			growRegion ((int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z + 1);
			
			//after growing, grab min, max values for comparison later.
			minVector = new Vector3(minX, minY, minZ);
			maxVector = new Vector3(maxX, maxY, maxZ);
			
			regionsMinMax[4,0] = minVector;
			regionsMinMax[4,1] = maxVector;
		}
		
		if ((int)gridCoordinates.z > 0 && voxelGrid [(int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z - 1] != null) {
			//clear array and reset min maxes on this voxel
			Array.Clear(visited, 0, visited.Length);
			minX = (int)gridCoordinates.x;
			maxX = (int)gridCoordinates.x;
			minY = (int)gridCoordinates.y;
			maxY = (int)gridCoordinates.y;
			minZ = (int)gridCoordinates.z - 1;
			maxZ = (int)gridCoordinates.z - 1;
			
			//deletion candidate set to visited, so cannot grow shape through this voxel.
			visited[(int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z] = true;
			
			//grow region from the neighbour.
			growRegion ((int)gridCoordinates.x, (int)gridCoordinates.y, (int)gridCoordinates.z - 1);
			
			//after growing, grab min, max values for comparison later.
			minVector = new Vector3(minX, minY, minZ);
			maxVector = new Vector3(maxX, maxY, maxZ);
			
			regionsMinMax[5,0] = minVector;
			regionsMinMax[5,1] = maxVector;
		}
		
		//after all the regions grown, compare them and if they are the same, then allow deletion

		for (int i = 0; i < 6; i++) {

			Debug.Log (regionsMinMax[i, 0] + " " + regionsMinMax[i, 1]);
			//Debug.Log (regionsMinMax[i, 0].Equals(regionsMinMax[i, 1]));
			Debug.Log ("-----------------");
			            
		}

		for (int i = 0; i < 6; i++) {
			
			if (regionsMinMax[i, 0] != Vector3.zero)
			{
				for (int j = 0; j < 6; j++){
					
					if (regionsMinMax[j, 0] != Vector3.zero){
						
						//if not equal
						if (!regionsMinMax[i,0].Equals(regionsMinMax[j,0]) && !regionsMinMax[i,1].Equals(regionsMinMax[j,1]))
							return false;
					}
				}
			}
		}

		
		return true;
		
		
	}
	
	void growRegion(int x, int y, int z){
		
		//been here already
		if (visited [x, y, z] == true)
			return;
		
		//nothing here
		if (voxelGrid [x, y, z] == null) {
			visited [x, y, z] = true;
			return;
		}

		visited [x, y, z] = true;
		
		//set max values
		if (x > maxX)
			maxX = x;
		
		if (x < minX)
			minX = x;
		
		if (y > maxY)
			maxY = y;
		
		if (y < minY)
			minY = y;
		
		if (z > maxZ)
			maxZ = z;
		
		if (z < minZ)
			minZ = z;

		if (x < DRAW_WIDTH -1)
			growRegion (x + 1, y, z);

		if (x > 0)
			growRegion (x - 1, y, z);

		if (y < DRAW_LENGTH -1)
			growRegion (x, y + 1, z);

		if (y > 0)
			growRegion (x, y - 1, z);

		if (z < DRAW_HEIGHT - 1)
			growRegion (x, y, z + 1);

		if (z > 0)
			growRegion (x, y, z - 1);
		
		return;

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