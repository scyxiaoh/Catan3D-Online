using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class Vertex : MonoBehaviour
{
	public bool isBuildable = false;
	public bool isOccupied = false;
	public bool isRealOccupied = false;
	public bool isLand = false;

	[HideInInspector]public GameObject thisVertex; 
	[HideInInspector]float[] pos = new float[2];                                      // The position of the vertex.
	public Hex[] adjacentHexes = new Hex[3];
	public Edge[] adjacentEdges = new Edge[3];


	public int id;
	public int buildingType = 0;
	private float delay = 0;
	public HarbourType harbourtype;

	public GameBoard gameBoard;

	public delegate void VertexBuildDelegate();
	public VertexBuildDelegate waitingBuildDelegate;
	public void RunWaitingBuildDelegate(){
		waitingBuildDelegate ();
	}

	private void Start(){
		gameBoard = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
	}

	private void Update()
	{
		if(delay > 0)
		{
			delay -= Time.deltaTime;
		}
		//rotate the selection marker when it's actived
		if (gameObject.transform.GetChild (0).gameObject.activeSelf) {
			gameObject.transform.GetChild (0).Rotate (new Vector3 (0, 0, 45) * Time.deltaTime);
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
		gameObject.transform.GetChild (0).gameObject.SetActive (true);
		Camera.main.gameObject.GetComponent<CameraController> ().locked = true;							//lock camera
		panelConfirmation.transform.position = Camera.main.WorldToScreenPoint(transform.position) + new Vector3(0,-20,0);
		bConfirm.SetActive (true);
		bAbort.SetActive (true);

		bConfirm.GetComponent<Button> ().onClick.AddListener (RunWaitingBuildDelegate);
		bAbort.GetComponent<Button> ().onClick.AddListener (delegate {
			gameObject.transform.GetChild (0).gameObject.SetActive (false);
			bConfirm.SetActive (false);
			bAbort.SetActive (false);
			bConfirm.GetComponent<Button> ().onClick.RemoveListener (RunWaitingBuildDelegate);
			Camera.main.gameObject.GetComponent<CameraController> ().locked = false;							//release camera
		});
	}
	public void setPosition(float x, float z)
	{
		transform.position = new Vector3(x, 0.03f, z);
		pos[0] = x;
		pos[1] = z;
	}

	public float[] getPosition()
	{
		return pos;
	}

	public void add(Hex hex)
	{
		for (int i = 0; i < 3; i++)
		{
			if (adjacentHexes[i] == null)
			{
				adjacentHexes[i] = hex;
				return;
			}
		}
	}

	public void add(Edge edge)
	{
		for (int i = 0; i < 3; i++)
		{
			if (adjacentEdges[i] == null)
			{
				adjacentEdges[i] = edge;
				return;
			}
		}
	}

	public void BuildSettlementRoundOne(){
		BuildSettlementSetUp ();
		RequestBuildRoadOnAdjacentEdges ();
	}

	public void BuildCityRoundTwo(){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		BuildSettlementSetUp ();
		UpgradeSettlementRoundTwo ();
		foreach (Hex h in adjacentHexes) {
			if (h != null) {
				localPlayer.OnResourcesChanged (h.Product (), 1);
			}
		}
		RequestBuildRoadOnAdjacentEdges ();
	}

	public void BuildCityWall(){
		int vertexIndex = gameBoard.junctions.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		if (localPlayer.resources [1] < 2 && !localPlayer.isEnginner) {
			GameObject.Find ("GameBoard").GetComponent<GameBoard> ().panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have insuffient resources.", null, null);
			return;
		} else {
			localPlayer.CmdBuildCityWall (vertexIndex);
		}
	}

	public void RequestBuildRoadOnAdjacentEdges(){
		foreach (Edge e in adjacentEdges) {
			if (e != null) {
				if (e.isBuildable && !e.isOccupied && e.isLand ) {
					e.gameObject.GetComponent<BoxCollider> ().enabled = true;
					e.gameObject.transform.GetChild (0).gameObject.SetActive (true);
					e.waitingBuildDelegate = e.buildRoad;
				}
			}
		}
		gameBoard.waitingDelegate = GameObject.Find("Local Player Panel").GetComponent<Player> ().EndTurn;
		GameObject.Find("GameBoard").GetComponent<GameBoard>().panelInfoBar.GetComponent<InfoBar> ().pushMessage ("Build a road!");
	}

	public void BuildSettlementSetUp(){
		int vertexIndex = gameBoard.junctions.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdBuildSettlement (vertexIndex, true);
		for (int i = 0; i < 3; i++) {
			if (adjacentEdges [i] != null) {
				if(adjacentEdges [i].isOccupied == false &&
					adjacentEdges [i].isOccupiedByShip == false){
					if (adjacentEdges [i].isLand) {
						adjacentEdges [i].isBuildable = true;
					}
					if (adjacentEdges [i].isCoast) {
						adjacentEdges [i].isShipBuildable = true;
					}
				}

			}
		}
		gameBoard.BuildVertexRequestEnd ();
	}

	public void BuildSettlement(){
		int vertexIndex = gameBoard.junctions.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdBuildSettlement (vertexIndex, false);
		for (int i = 0; i < 3; i++) {
			if (adjacentEdges [i] != null) {
				if(adjacentEdges [i].isOccupied == false &&
					adjacentEdges [i].isOccupiedByShip == false){
					if (adjacentEdges [i].isLand) {
						adjacentEdges [i].isBuildable = true;
					}
					if (adjacentEdges [i].isCoast) {
						adjacentEdges [i].isShipBuildable = true;
					}
				}

			}
		}
		gameBoard.BuildVertexRequestEnd ();
	}

	public void UpgradeSettlementRoundTwo(){
		int vertexIndex = gameBoard.junctions.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdUpgradeSettlement (vertexIndex);
	}

	public void UpgradeSettlement(){
		//3 ore 2 grain
		//max 4 per player
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		if ((localPlayer.resources [3] < 2 || localPlayer.resources [4] < 3) && !localPlayer.isMedicine ||
		    ((localPlayer.resources [3] < 1 || localPlayer.resources [4] < 2) && localPlayer.isMedicine)) {
			GameObject.Find ("GameBoard").GetComponent<GameBoard> ().panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have insuffient resources.", null, null);
			return;
		}

		//check if max number was added. 
		int villageCounter = 0;
		List<Village> villages = localPlayer.villages;
		foreach (var village in villages) {
			if (village.vt == VillageType.City)
				villageCounter++;
		}
		if (villageCounter > 3){
			GameObject.Find("GameBoard").GetComponent<GameBoard>().panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have reached your city limit! (4)",null,null);
			return;
		}

		int vertexIndex = gameBoard.junctions.IndexOf (gameObject);
		GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdUpgradeSettlement (vertexIndex);
	}


	public void UpgradeCityToMetropole(int type){
		int vertexIndex = gameBoard.junctions.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdUpgradeCityToMetropole (vertexIndex, type);
		gameBoard.BuildVertexRequestEnd ();
	}

	public void DowngradeCity(){
		int vertexIndex = gameBoard.junctions.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdDowngradeCity (vertexIndex);
		gameBoard.BuildVertexRequestEnd ();
		gameBoard.EnableAllKnightsAndVillagesSelection (false);
		localPlayer.CmdResponseToServer ();
	}

	public void PurchaseKnight(){
		int vertexIndex = gameBoard.junctions.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdPurchaseKnight (vertexIndex);
		gameBoard.panelActions.transform.GetChild (1).GetChild (4).gameObject.SetActive (false);
		gameBoard.BuildVertexRequestEnd ();
	}

	public void SetKnightActive(bool activated){
		int vertexIndex = gameBoard.junctions.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		if (localPlayer.resources [3] < 1) {
			GameObject.Find("GameBoard").GetComponent<GameBoard>().panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have insuffient resources.",null,null);
			return;
		} else {
			localPlayer.CmdSetKnightActive (vertexIndex, activated);
		}
	}

	public void PromoteKnight(bool activated){
		int vertexIndex = gameBoard.junctions.IndexOf (gameObject);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		if ((localPlayer.resources [2] < 0 || localPlayer.resources [4] < 0) && (localPlayer.isSmith == 0)) {
			GameObject.Find ("GameBoard").GetComponent<GameBoard> ().panelInfo.GetComponent<InfoPanel> ().pushMessage ("You have insuffient resources.", null, null);
			return;
		} else {
			localPlayer.CmdUpgradeKnight (vertexIndex, activated);
		}
	}
		
	public void StealResource (bool isMyTurn){
		int targetResourceSum = transform.GetChild (1).gameObject.GetComponent<Village> ().owner.resourceSum;
		int resourceIndexStolen = Random.Range (0, targetResourceSum);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		Player targetPlayer = transform.GetChild (1).gameObject.GetComponent<Village> ().owner;
		localPlayer.CmdStealResourece (resourceIndexStolen, l.IndexOf(targetPlayer), isMyTurn);
		gameBoard.BuildVertexRequestEnd ();
		if (!isMyTurn) {
			gameBoard.EnableAllKnightsAndVillagesSelection (false);
		}
	}

	public void ChaseAwayRobber(){
		GameObject currentRobberAt = gameBoard.robber.transform.parent.gameObject;
		foreach (Hex h in adjacentHexes) {
			if (h != null && h.gameObject == currentRobberAt) {
				h.GetComponent<CircleCollider2D> ().enabled = true;
				h.GetComponent<Hex> ().waitingDelegate = delegate {
					gameBoard.HexActionRequestEnd();
					h.GetComponent<CircleCollider2D> ().enabled = false;
					gameBoard.RequestChaseAwayRobber();
					GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdSetKnightActive (gameBoard.junctions.IndexOf (gameObject), false);
					transform.GetChild (1).gameObject.GetComponent<Knight> ().hasMovedThisTurn = true;
				};
			}
		}
	}
}
