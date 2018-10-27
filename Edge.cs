using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Edge : MonoBehaviour
{
	[HideInInspector]public GameObject thisEdge;
	[HideInInspector]public bool isLand = false;
	[HideInInspector]private float[] pos = new float[2];                                      // The position of the vertex.
	[HideInInspector]public Hex[] adjacentHexes = new Hex[2];
	public Vertex[] adjacentVertices = new Vertex[2];

	public int ownerID = -1;
	public bool isBuildable = false;
	public bool isShipBuildable = false;
	public bool isOccupied = false;
	public bool isOccupiedByShip = false;
	public bool isCoast = false; //we need this 
	public EdgeUnit myEdgeUnit;	// the road on it (if there is one)

	public GameBoard gameBoard;

	public delegate void EdgeBuildDelegate();
	public EdgeBuildDelegate waitingBuildDelegate;
	public void RunWaitingBuildDelegate(){
		waitingBuildDelegate ();
	}

	private void Start(){
		gameBoard = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
	}

	private void Update(){
		if (gameObject.transform.GetChild (1).gameObject.activeSelf) {
			gameObject.transform.GetChild (1).Rotate (new Vector3 (0, 0, 45) * Time.deltaTime);
		}
	}

	void OnMouseEnter(){
		gameObject.transform.localScale = new Vector3 (1.3f, 1.3f, 1.3f);
	}

	void OnMouseExit(){
		gameObject.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
	}

	void OnMouseUp(){
		GameObject panelConfirmation = GameObject.Find ("panelBuildConfirmation");
		GameObject bConfirm = panelConfirmation.transform.GetChild (0).gameObject;
		GameObject bAbort = panelConfirmation.transform.GetChild (1).gameObject;
		if (bAbort.activeSelf) {
			bAbort.GetComponent<Button> ().onClick.Invoke ();
			bAbort.GetComponent<Button> ().onClick.RemoveAllListeners ();
		}
		gameObject.transform.GetChild (1).gameObject.SetActive (true);
		Camera.main.gameObject.GetComponent<CameraController> ().locked = true;
		panelConfirmation.transform.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0,-20,0);
		bConfirm.SetActive (true);
		bAbort.SetActive (true);

		bConfirm.GetComponent<Button> ().onClick.AddListener (RunWaitingBuildDelegate);
		bAbort.GetComponent<Button> ().onClick.AddListener (delegate {
			gameObject.transform.GetChild (1).gameObject.SetActive (false);
			bConfirm.SetActive (false);
			bAbort.SetActive (false);
			bConfirm.GetComponent<Button> ().onClick.RemoveListener (RunWaitingBuildDelegate);
			Camera.main.gameObject.GetComponent<CameraController> ().locked = false;	
		});
	}
		
	public void setPosition(float x, float z, int o)
	{
		transform.position = new Vector3(x, 0, z);
		this.transform.Rotate(new Vector3(0, 90 + o * 60, 0));
		pos[0] = x;
		pos[1] = z;
	}
		
	public float[] getPosition()
	{
		return pos;
	}

	public void add(Hex hex)
	{
		for (int i = 0; i < 2; i++)
		{
			if (adjacentHexes[i] == null)
			{
				adjacentHexes[i] = hex;
				return;
			}
		}
	}

	public void add(Vertex vertex)                       // Allows to add edges to the edge list.
	{
		for (int i = 0; i < 2; i++)
		{
			if (adjacentVertices[i] == null)
			{
				adjacentVertices[i] = vertex;
				return;
			}
		}
	}

	public void buildRoad(){
		int edgeIndex = gameBoard.edges.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdBuildRoad (edgeIndex);
		/*
		foreach (Vertex v in adjacentVertices) {
			if (v != null && !v.isOccupied && !v.isBuildable) {
				v.isBuildable = true;
			}
		}
		*/
		gameBoard.BuildEdgeRequestEnd ();
		gameBoard.RunWaitingDelegate ();
	
	}

	public void buildShip()
	{
		int edgeIndex = gameBoard.edges.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdBuildShip (edgeIndex);

		gameBoard.BuildEdgeRequestEnd ();
		gameBoard.RunWaitingDelegate ();
	}
		
	public void StealResource (){
		int targetResourceSum = transform.GetChild (2).gameObject.GetComponent<EdgeUnit> ().owner.resourceSum;
		int resourceIndexStolen = Random.Range (0, targetResourceSum);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		Player targetPlayer = transform.GetChild (2).gameObject.GetComponent<EdgeUnit> ().owner;
		localPlayer.CmdStealResourece (resourceIndexStolen, l.IndexOf(targetPlayer), false);
		gameBoard.BuildEdgeRequestEnd ();
		gameBoard.EnableAllKnightsAndVillagesSelection (false);
	}

	public void MoveShip(){

	}
}
