using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hex : MonoBehaviour
{
	float[] pos = new float[2];                  // The position of the center of the Hex.
	public Edge[] adjacentEdges = new Edge[6];          // The edges/roads around the hex.
	public Vertex[] adjacentVertices = new Vertex[6];   // The vertices around the hex.


	public int hexNumber = -1;
	public int resourceType = -1;
	public bool isLand = false;
	private bool isSeleted = false;

	public GameBoard gameBoard;

	public delegate void HexDelegate();
	public HexDelegate waitingDelegate;
	public void RunWaitingDelegate(){
		isSeleted = false;
		waitingDelegate ();
	}

	private void Start(){
		gameBoard = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
	}

	private void Update(){
		if (gameObject.transform.GetChild (0).gameObject.activeSelf) {
			gameObject.transform.GetChild (0).Rotate (new Vector3 (0, 0, 45) * Time.deltaTime);
		}
	}

	private Edge getEdge(int i)                         // The getter for the edges.
	{
		if(i > -1 && i < 6)
		{
			return adjacentEdges[i];
		}
		return null;
	}

	public void add(Vertex newVertex)                       // Allows to add edges to the edge list.
	{
		for (int i = 0; i < 6; i++)
		{
			if (adjacentVertices[i] == null)
			{
				adjacentVertices[i] = newVertex;
				return;
			}
		}
	}

	public void add(Edge edge)
	{
		for (int i = 0; i < 6; i++)
		{
			if (adjacentEdges[i] == null)
			{
				adjacentEdges[i] = edge;
				return;
			}
		}
	}

	public void setPosition(float x, float z)
	{
		transform.position = new Vector3(x, 0, z);
		//this.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
		pos[0] = x;
		pos[1] = z;
	}

	public float[] getPosition()
	{
		return pos;
	}

	public void linkVertices(List<GameObject> junctions)
	{
		for (int i = 0; i < junctions.Count; i++)
		{
			float[] vpos = junctions[i].GetComponent<Vertex> ().getPosition();
			if ((vpos[0] - pos[0]) * (vpos[0] - pos[0]) + (vpos[1] - pos[1]) * (vpos[1] - pos[1]) < 1.1)
			{
				add(junctions[i].GetComponent<Vertex> ());
				junctions[i].GetComponent<Vertex> ().add(this);
				if (isLand && !junctions[i].GetComponent<Vertex> ().isBuildable)
				{
					junctions[i].GetComponent<Vertex> ().isBuildable = true;
				}
			}
		}
	}

	public void linkEdges(List<GameObject> roads)
	{
		for (int i = 0; i < roads.Count; i++)
		{
			float[] rpos = roads[i].GetComponent<Edge> ().getPosition();
			if ((rpos[0] - pos[0]) * (rpos[0] - pos[0]) + (rpos[1] - pos[1]) * (rpos[1] - pos[1]) < 1.1)
			{
				add(roads[i].GetComponent<Edge> ());
				roads[i].GetComponent<Edge> ().add(this);
				if (isLand && !roads[i].GetComponent<Edge> ().isLand)
				{
					roads[i].GetComponent<Edge> ().isLand = true;
				}
			}
		}
	}
		

	public ResourceType Product(){
		switch (resourceType) {
		case -1: return ResourceType.None;
		case 0: return ResourceType.None;
		case 1: return ResourceType.Wool;
		case 2: return ResourceType.Lumber;
		case 3: return ResourceType.Ore;
		case 4: return ResourceType.Brick;
		case 5: return ResourceType.Grain;
		default:return ResourceType.Any;
		}
	}

	public void OnMouseOver()
	{
		this.transform.GetChild (0).gameObject.SetActive (true);
	}

	public void OnMouseExit()
	{
		if (!isSeleted) {
			this.transform.GetChild(0).gameObject.SetActive (false);
		}
	}

	public void OnMouseUp()
	{
		GameObject panelConfirmation = GameObject.Find ("panelBuildConfirmation");
		GameObject bConfirm = panelConfirmation.transform.GetChild (0).gameObject;
		GameObject bAbort = panelConfirmation.transform.GetChild (1).gameObject;
		if (bAbort.activeSelf) {
			bAbort.GetComponent<Button> ().onClick.Invoke ();
			bAbort.GetComponent<Button> ().onClick.RemoveAllListeners ();
		}
		isSeleted = true;
		gameObject.transform.GetChild (0).gameObject.SetActive (true);
		Camera.main.gameObject.GetComponent<CameraController> ().locked = true;							//lock camera
		panelConfirmation.transform.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0,-20,0);
		bConfirm.SetActive (true);
		bAbort.SetActive (true);

		bConfirm.GetComponent<Button> ().onClick.AddListener (RunWaitingDelegate);
		bAbort.GetComponent<Button> ().onClick.AddListener (delegate {
			gameObject.transform.GetChild (0).gameObject.SetActive (false);
			bConfirm.SetActive (false);
			bAbort.SetActive (false);
			bConfirm.GetComponent<Button> ().onClick.RemoveListener (RunWaitingDelegate);
			Camera.main.gameObject.GetComponent<CameraController> ().locked = false;							//release camera
			isSeleted = false;
		});
	}

	public void MoveRobber(bool isMyTurn){
		int hexIndex = gameBoard.tiles.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdMoveRobber (hexIndex);
	
		int numOtherPlayerVillages = 0;
		foreach (Vertex v in adjacentVertices) {
			if (v != null && v.transform.childCount > 1 && (v.transform.GetChild (1).gameObject.GetComponent<Village> () != null)) {
				Village village = v.transform.GetChild (1).gameObject.GetComponent<Village> ();
				if (!village.owner.isLocalPlayer) {
					v.gameObject.GetComponent<MeshRenderer> ().enabled = true;
					v.gameObject.GetComponent<CircleCollider2D> ().enabled = true;
					v.waitingBuildDelegate = delegate {
						v.StealResource (isMyTurn);
					};
					numOtherPlayerVillages++;
				}
			}
		}
		gameBoard.HexActionRequestEnd ();
		gameBoard.EnableAllKnightsAndVillagesSelection (false);
		if (numOtherPlayerVillages == 0) {
			if (!isMyTurn) {
				localPlayer.CmdResponseToServer ();
			} else {
				gameBoard.EnableAllKnightsAndVillagesSelection (true);
				gameBoard.EnablePrimaryActionPanel (true);
			}
		}
	}

	public void MovePirate(){
		int hexIndex = gameBoard.tiles.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdMovePirate (hexIndex);
		int numOtherPlayerShips = 0;
		foreach (Edge e in adjacentEdges) {
			if (e != null && e.transform.childCount > 2 && (e.transform.GetChild (2).gameObject.GetComponent<EdgeUnit> () != null)) {
				EdgeUnit ship = e.transform.GetChild (2).gameObject.GetComponent<EdgeUnit> ();
				if (!ship.owner.isLocalPlayer) {
					e.gameObject.GetComponent<BoxCollider> ().enabled = true;
					e.transform.GetChild (0).gameObject.SetActive (true);
					e.gameObject.GetComponent<Edge> ().waitingBuildDelegate = e.gameObject.GetComponent<Edge> ().StealResource;
					numOtherPlayerShips++;
				}
			}
		}
		gameBoard.HexActionRequestEnd ();
		gameBoard.EnableAllKnightsAndVillagesSelection (false);
		if (numOtherPlayerShips == 0) {
			localPlayer.CmdResponseToServer ();
		}
	}

	public void DoBishop(){
		int hexIndex = gameBoard.tiles.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdMoveRobber (hexIndex);

		List<Player> hasStolen = new List<Player> ();
		hasStolen.Add (localPlayer);
		foreach (Vertex v in adjacentVertices) {
			if (v != null && v.transform.childCount > 1 && (v.transform.GetChild (1).gameObject.GetComponent<Village> () != null)) {
				Village village = v.transform.GetChild (1).gameObject.GetComponent<Village> ();
				if (!hasStolen.Contains(village.owner)) {
					v.StealResource (true);
					hasStolen.Add (village.owner);
				}
			}
		}
		gameBoard.HexActionRequestEnd ();
		gameBoard.EnableAllKnightsAndVillagesSelection (true);
	}

	public void MoveMerchant(){
		int hexIndex = gameBoard.tiles.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdMoveMerchant (hexIndex);
		gameBoard.HexActionRequestEnd ();
		gameBoard.EnablePrimaryActionPanel (true);
	}
		
}
