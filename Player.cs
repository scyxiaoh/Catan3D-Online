using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;


public class Player : NetworkBehaviour {


	[SyncVar]public string playerName = "";
	[SyncVar]public Color playerColor = Color.white;
	[SyncVar]public int numOfCityWall;
	[SyncVar]public bool isReady = false;
	[SyncVar]public bool hasMerchant = false;
	[SyncVar]public bool hasAlchemist = false;
	[SyncVar(hook = "OnVpChanged")]
	public int vP = 0;
	[SyncVar(hook = "OnResourceSumChanged")]
	public int resourceSum = 0;
	[SyncVar(hook = "OnProgressCardSumaanged")]
	public int progressCardSum = 0;
	[SyncVar(hook = "OnDefendCountChanged")]
	public int defendCount = 0;
	[SyncVar(hook = "OnLongestRoadChanged")]
	public int longestRoad = 0;
	[SyncVar(hook = "OnFCTradeLvlChanged")]
	public int fcTradeLvl = 1;
	[SyncVar(hook = "OnFCPoliticsLvlChanged")]
	public int fcPoliticsLvl = 1;
	[SyncVar(hook = "OnFCScienceLvlChanged")]
	public int fcScienceLvl = 1;


	public List<int> resources = new List<int>();
	public List<int> tradeRatios = new List<int>();
	public List<Text> resourceDisplay = new List<Text>();
	public List<ProgressCard> pgCard = new List<ProgressCard>();
	public List<Village> villages = new List<Village>();
	public List<Knight> knights = new List<Knight> ();
	public List<EdgeUnit> roads = new List<EdgeUnit>();

	public Text vpDisplay, resDisplay, pcDisplay, defDisplay, roadDisplay;
	public Image fcTradeDisplay, fcPoliticsDisplay, fcScienceDisplay;

	// all TODO
	public bool isCrane = false;
	public bool isEnginner = false;
	public bool isMedicine = false;
	public int roadBuilding = 0;
	public int isSmith = 0;
	public int isWarLord = 0;

	public bool masterMerchant;
	public bool resourceMonopoly;
	public bool tradeMonopoly;
	public bool merchantFleet;
	public bool commercialHarbor;

	public void Start()
	{

		PlayerList._instance.AddPlayer(this);
		if (isLocalPlayer)
		{
			SetupLocalPlayer();
		}
		else
		{
			SetupOtherPlayer();
		}
		OnVpChanged (vP);
		OnResourceSumChanged (resourceSum);
		OnProgressCardSumChanged (progressCardSum);
		OnDefendCountChanged (defendCount);
		OnLongestRoadChanged (longestRoad);
		OnFCTradeLvlChanged(1);
		OnFCPoliticsLvlChanged(1);
		OnFCScienceLvlChanged(1);
		//set up resources and traderatios
		for (int i = 0; i < 8; i++){
			resources.Add (0);
			tradeRatios.Add (4);
		}
		isCrane = false;


	}

	public void Update(){
		// to debug
		if (!isLocalPlayer) {
			return;
		}
		if (Input.GetKeyDown (KeyCode.A)) {
		} 
		if (Input.GetKeyDown (KeyCode.S)) {
		} 
		if (Input.GetKeyDown (KeyCode.D)) {
		} 
		if (Input.GetKeyDown (KeyCode.Z)) {
			for (int i = 0; i < 8; i++) {
				OnResourcesChanged ((ResourceType)i,1);
			}
		} 
		if (Input.GetKeyDown (KeyCode.X)) {
		} 
		if (Input.GetKeyDown (KeyCode.C)) {
		}
	}
	void SetupLocalPlayer(){
		gameObject.name="Local Player Panel";
		gameObject.transform.GetChild (1).gameObject.SetActive (true);
		gameObject.transform.position = new Vector3 (-29,54,0);
		// set name
		gameObject.transform.GetChild (1).GetChild (7).GetComponent<Text> ().text = playerName;
		// set color
		if (playerColor == Color.red) {
			gameObject.transform.GetChild (1).GetChild (1).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("UI/playername_red");
		} else if (playerColor == Color.green) {
			gameObject.transform.GetChild (1).GetChild (1).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("UI/playername_green");
		} else if (playerColor == Color.gray) {
			gameObject.transform.GetChild (1).GetChild (1).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("UI/playername_gray");
		}
		// set text components
		vpDisplay = gameObject.transform.GetChild (1).GetChild (2).GetComponent<Text> ();
		resDisplay = gameObject.transform.GetChild (1).GetChild (3).GetComponent<Text> ();
		pcDisplay = gameObject.transform.GetChild (1).GetChild (4).GetComponent<Text> ();
		defDisplay = gameObject.transform.GetChild (1).GetChild (5).GetComponent<Text> ();
		roadDisplay = gameObject.transform.GetChild (1).GetChild (6).GetComponent<Text> ();
		for (int i = 0; i < 8; i++) {
			resourceDisplay.Add (GameObject.Find("panelResources").transform.GetChild(i).gameObject.GetComponent<Text>());
		}
		CmdSetReady(true);
	}

	void SetupOtherPlayer(){
		gameObject.transform.GetChild (0).gameObject.SetActive (true);
		// set name
		gameObject.transform.GetChild (0).GetChild (10).GetComponent<Text>().text = playerName;
		// set color
		if (playerColor == Color.blue) {
			gameObject.transform.GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("UI/player_VP_blue");
		} else if (playerColor == Color.green) {
			gameObject.transform.GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("UI/player_VP_green");
		} else if (playerColor == Color.gray) {
			gameObject.transform.GetChild (0).GetChild (0).GetComponent<Image> ().sprite = Resources.Load<Sprite> ("UI/player_VP_gray");
		}
		// set text components
		vpDisplay = gameObject.transform.GetChild (0).GetChild (5).GetComponent<Text> ();
		resDisplay = gameObject.transform.GetChild (0).GetChild (6).GetComponent<Text> ();
		pcDisplay = gameObject.transform.GetChild (0).GetChild (7).GetComponent<Text> ();
		defDisplay = gameObject.transform.GetChild (0).GetChild (8).GetComponent<Text> ();
		roadDisplay = gameObject.transform.GetChild (0).GetChild (9).GetComponent<Text> ();
		fcTradeDisplay = gameObject.transform.GetChild(0).GetChild(11).GetChild(0).GetComponent<Image>();
		fcPoliticsDisplay = gameObject.transform.GetChild(0).GetChild(11).GetChild(1).GetComponent<Image>();
		fcScienceDisplay = gameObject.transform.GetChild(0).GetChild(11).GetChild(2).GetComponent<Image>();
	}

	public void OnVpChanged(int newVP){
		vP = newVP;
		vpDisplay.text = vP.ToString ();
	}
	public void OnResourceSumChanged(int newSum){
		resourceSum = newSum;
		resDisplay.text = resourceSum.ToString ();
		Color toColor = (resourceSum > 7 + numOfCityWall * 2)? Color.red : Color.white;
		resDisplay.color = toColor;
	}
	public void OnProgressCardSumChanged(int newSum){
		progressCardSum = newSum;
		pcDisplay.text = progressCardSum.ToString ();
	}
	public void OnDefendCountChanged(int newCount){
		defendCount = newCount;
		defDisplay.text = defendCount.ToString ();
	}
	public void OnLongestRoadChanged(int newLength){
		longestRoad = newLength;
		roadDisplay.text = longestRoad.ToString ();
	}
	public void OnFCTradeLvlChanged(int newlvl)
	{
		fcTradeLvl = newlvl;
		if (isLocalPlayer && newlvl < 7 && newlvl > 1)
		{
			GameObject.Find("panelFlipChart").transform.GetChild(0).GetChild(newlvl - 2).gameObject.SetActive(false);
			GameObject.Find("panelFlipChart").transform.GetChild(0).GetChild(newlvl - 1).gameObject.SetActive(true);
			GameObject.Find ("GameBoard").GetComponent<GameBoard> ().RenewImprovementsAvailability();
		}
		else if (!isLocalPlayer)
		{
			if (newlvl == 2)
			{
				fcTradeDisplay.gameObject.SetActive(true);
				fcTradeDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_2");
			}
			else if (newlvl == 3)
			{
				fcTradeDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_3");
			}
			else if (newlvl == 4)
			{
				fcTradeDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_4");
			}
			else if (newlvl == 5)
			{
				fcTradeDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_5");
			}
			else if (newlvl == 6)
			{
				fcTradeDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_6");
			}
		}


		//Metropole Count on Sever?????
	}
	public void OnFCPoliticsLvlChanged(int newlvl)
	{
		fcPoliticsLvl = newlvl;
		if (isLocalPlayer && newlvl < 7 && newlvl > 1)
		{
			GameObject.Find("panelFlipChart").transform.GetChild(1).GetChild(newlvl - 2).gameObject.SetActive(false);
			GameObject.Find("panelFlipChart").transform.GetChild(1).GetChild(newlvl - 1).gameObject.SetActive(true);
			GameObject.Find ("GameBoard").GetComponent<GameBoard> ().RenewImprovementsAvailability();
		}else if (!isLocalPlayer) {
			if (newlvl == 2)
			{	
				fcPoliticsDisplay.gameObject.SetActive(true);
				fcPoliticsDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_2");
			}
			else if (newlvl == 3)
			{
				fcPoliticsDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_3");
			}
			else if (newlvl == 4)
			{
				fcPoliticsDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_4");
			}
			else if (newlvl == 5)
			{
				fcPoliticsDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_5");
			}
			else if (newlvl == 6)
			{
				fcPoliticsDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_6");
			}
		}



		//Metropole Count on Sever?????
	}
	public void OnFCScienceLvlChanged(int newlvl)
	{
		fcScienceLvl = newlvl;
		if (isLocalPlayer && newlvl < 7 && newlvl > 1) {
			GameObject.Find ("panelFlipChart").transform.GetChild (2).GetChild (newlvl - 2).gameObject.SetActive (false);
			GameObject.Find ("panelFlipChart").transform.GetChild (2).GetChild (newlvl - 1).gameObject.SetActive (true);
			GameObject.Find ("GameBoard").GetComponent<GameBoard> ().RenewImprovementsAvailability();
		} else if (!isLocalPlayer) {
			if (newlvl == 2)
			{
				fcScienceDisplay.gameObject.SetActive(true);
				fcScienceDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_2");
			}
			else if (newlvl == 3)
			{
				fcScienceDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_3");
			}
			else if (newlvl == 4)
			{
				fcScienceDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_4");
			}
			else if (newlvl == 5)
			{
				fcScienceDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_5");
			}
			else if (newlvl == 6)
			{
				fcScienceDisplay.sprite = Resources.Load<Sprite>("UI/dice_flipChart_6");
			}
		}

		//Metropole Count on Sever?????
	}





	public void OnResourcesChanged(ResourceType t, int i) {
		int index = 0;
		if (t == ResourceType.Lumber) {
			index = 0;
		} else if (t == ResourceType.Brick) {
			index = 1;
		} else if (t == ResourceType.Wool) {
			index = 2;
		} else if (t == ResourceType.Grain) {
			index = 3;
		} else if (t == ResourceType.Ore) {
			index = 4;
		} else if (t == ResourceType.Paper) {
			index = 5;
		} else if (t == ResourceType.Cloth) {
			index = 6;
		} else if (t == ResourceType.Coin) {
			index = 7;
		} else {
			return;
		}
		resources [index] += i;
		resourceDisplay [index].text = resources [index].ToString();
		CmdResourceSumChange (i);
	}

	public int getTradeRatio(ResourceType t) {
		int ratio = 0;
		if (t == ResourceType.Lumber) {
			ratio = tradeRatios [0];
		} else if (t == ResourceType.Brick) {
			ratio = tradeRatios [1];
		} else if (t == ResourceType.Wool) {
			ratio = tradeRatios [2];
		} else if (t == ResourceType.Grain) {
			ratio = tradeRatios [3];
		} else if (t == ResourceType.Ore) {
			ratio = tradeRatios [4];
		} else if (t == ResourceType.Paper) {
			ratio = tradeRatios [5];
		} else if (t == ResourceType.Cloth) {
			ratio = tradeRatios [6];
		} else if (t == ResourceType.Coin) {
			ratio = tradeRatios [7];
		}
		return ratio;
	}

	public void processTrade(List<int> resourcesOffered){
		for (int i = 0; i < 8; i++) {
			OnResourcesChanged ((ResourceType)Enum.Parse(typeof(ResourceType),i.ToString()),-resourcesOffered[i]);
		}
	}

	[TargetRpc]
	public void TargetTradeRatioChange(NetworkConnection c, int type, int num){
		tradeRatios [type] = num;
	}

	[Command]
	public void CmdGiveTradeOffer(int[] resourcesOffered){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList>()._players;
		int indexOfferedBy = l.IndexOf(this);
		for (int i = 0; i < l.Count; i++) {
			if (i != indexOfferedBy) {
				TargetReceiveTradeOffer (l [i].connectionToClient, resourcesOffered, indexOfferedBy);
			}
		}
	}
	[Command]
	public void CmdResponseTradeOffer(int[] resourcesOffered, int toIndex){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList>()._players;
		int indexResponsedBy = l.IndexOf(this);
		TargetReceiveOfferResponse (l [toIndex].connectionToClient, resourcesOffered, indexResponsedBy);
	}

	[Command]
	public void CmdAcceptTradeOffer(int[] resourcesOffered, int toIndex){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList>()._players;
		int[] resourcesDesired = new int[resourcesOffered.Length];
		for (int i = 0; i < resourcesOffered.Length; i++) {
			resourcesDesired [i] = -resourcesOffered [i];
		}
		TargetProcessTrade (this.connectionToClient, resourcesDesired);
		TargetProcessTrade (l [toIndex].connectionToClient, resourcesOffered);
	}

	[Command]
	public void CmdStealResourece(int resourceIndexStolen, int targetPlayerIndex, bool isHisTurn){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		l[targetPlayerIndex].TargetResourceStolen (l [targetPlayerIndex].connectionToClient, resourceIndexStolen, l.IndexOf (this), isHisTurn);
	}
	[Command]
	public void CmdResourceChange(int targetPlayerIndex, int resIndex, int change){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		l[targetPlayerIndex].TargetResourceChange (l[targetPlayerIndex].connectionToClient, resIndex, change);
	}

	[Command]
	public void CmdNotifyHasAlchemist(bool check){
		hasAlchemist = check;
	}

	[TargetRpc]
	public void TargetReceiveTradeOffer(NetworkConnection c, int[] resourcesOffered, int indexOfferedBy){
		GameBoard board = GameObject.Find ("GameBoard").gameObject.GetComponent<GameBoard> ();
		board.panelTrade.GetComponent<TradeManager> ().ReceiveTradeOffer (resourcesOffered, indexOfferedBy);
	}

	[TargetRpc]
	public void TargetReceiveOfferResponse(NetworkConnection c, int[] resourcesOffered, int indexOfferedBy){
		GameBoard board = GameObject.Find ("GameBoard").gameObject.GetComponent<GameBoard> ();
		board.panelTrade.GetComponent<TradeManager> ().ReceiveOfferResponse (resourcesOffered, indexOfferedBy);
	}

	[TargetRpc]
	public void TargetProcessTrade(NetworkConnection c, int[] resourcesOffered){
		List<int> newResourcesList = new List<int> ();
		for (int i = 0; i < resourcesOffered.Length; i++) {
			newResourcesList.Add (resourcesOffered [i]);
		}
		GameObject.Find ("Local Player Panel").GetComponent<Player> ().processTrade (newResourcesList);
	}

	[Command]
	public void CmdRequestDiscardResources(int playerIndex, int quantity){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		l[playerIndex].TargetRequestDiscardResources (l[playerIndex].connectionToClient, quantity);
	}
	[Command]
	public void CmdPlayedWeddingCard(){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		foreach (Player p in l) {
			if (p != this && p.vP > this.vP) {
				p.TargetRequestDiscardResourcesToPlayer (p.connectionToClient, 2, l.IndexOf (this));
			}
		}
	}
	[Command]
	public void CmdPlayedResourceMonopoly(int resourceType){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		RpcResourceMonopolied (resourceType, l.IndexOf (this));
		RpcAnnounce(this.playerName +" decides to monopolize " + ((ResourceType)Enum.Parse (typeof(ResourceType), resourceType.ToString ())).ToString() +".");
	}

	[Command]
	public void CmdPlayedTradeMonopoly(int resourceType) {
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		RpcTradeMonopolied (resourceType, l.IndexOf (this));
		RpcAnnounce(this.playerName +" decides to monopolize " + ((ResourceType)Enum.Parse (typeof(ResourceType), resourceType.ToString ())).ToString() +".");
	}

	[Command]
	public void CmdSwapNumberTokens(int firstIndex, int secondIndex){
		RpcSwapNumberTokens (firstIndex, secondIndex);
	}

	[ClientRpc]
	public void RpcSwapNumberTokens(int firstIndex, int secondIndex){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		board.tiles [firstIndex].transform.GetChild (2).SetParent (board.tiles [secondIndex].transform,false);
		board.tiles [secondIndex].transform.GetChild (2).SetParent (board.tiles [firstIndex].transform,false);
		int temp = board.tiles [firstIndex].GetComponent<Hex> ().hexNumber;
		board.tiles [firstIndex].GetComponent<Hex> ().hexNumber = board.tiles [secondIndex].GetComponent<Hex> ().hexNumber;
		board.tiles [secondIndex].GetComponent<Hex> ().hexNumber = temp;
	}

	[ClientRpc]
	public void RpcTradeMonopolied(int resourceType, int byPlayerIndex){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		if (resources [resourceType] >= 1) {
			OnResourcesChanged ((ResourceType)Enum.Parse (typeof(ResourceType), resourceType.ToString ()), -1);
			CmdResourceChange(byPlayerIndex,resourceType, 1);
		}
	}

	[ClientRpc]
	public void RpcResourceMonopolied(int resourceType, int byPlayerIndex){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		if (resources [resourceType] >= 2) {
			OnResourcesChanged ((ResourceType)Enum.Parse (typeof(ResourceType), resourceType.ToString ()), -2);
			CmdResourceChange(byPlayerIndex,resourceType, 2);
		} else {
			CmdResourceChange(byPlayerIndex,resourceType, resources [resourceType]);
			OnResourcesChanged ((ResourceType)Enum.Parse (typeof(ResourceType), resourceType.ToString ()), -resources [resourceType]);
		}
	}

	[TargetRpc]
	public void TargetRequestDiscardResources(NetworkConnection c, int resourcesSumRequired){
		GameBoard board = GameObject.Find ("GameBoard").gameObject.GetComponent<GameBoard> ();
		board.panelDiscard.GetComponent<DiscardManager> ().resourceSumRequired = resourcesSumRequired;
		board.panelInfo.GetComponent<InfoPanel> ().pushMessage ("You must discard " + resourcesSumRequired + " resources or commidities.",
			board.panelDiscard.GetComponent<DiscardManager> ().RequestDiscardResourceToBank,
			board.panelDiscard.GetComponent<DiscardManager> ().RequestDiscardResourceToBank);
	}

	[TargetRpc]
	public void TargetRequestDiscardResourcesToPlayer(NetworkConnection c, int resourcesSumRequired, int targetPlayerIndex){
		GameBoard board = GameObject.Find ("GameBoard").gameObject.GetComponent<GameBoard> ();
		Player targetPlayer = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players [targetPlayerIndex];
		board.panelDiscard.GetComponent<DiscardManager> ().resourceSumRequired = resourcesSumRequired;
		board.panelInfo.GetComponent<InfoPanel> ().pushMessage ("You must give " + resourcesSumRequired + " resources or commidities to " + targetPlayer.playerName + ".",
			delegate {
				board.panelDiscard.GetComponent<DiscardManager> ().RequestDiscardResourceToPlayer (targetPlayerIndex);
			},
			delegate {
				board.panelDiscard.GetComponent<DiscardManager> ().RequestDiscardResourceToPlayer (targetPlayerIndex);
			});
	}

	[TargetRpc]
	public void TargetResourceStolen(NetworkConnection c, int resourceIndexStolen, int indexStolenBy, bool isHisTurn){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		int indexRemaining = resourceIndexStolen;
		for (int i = 0; i < resources.Count; i++) {
			for (int j = 0; j < resources [i]; j++) {
				if (indexRemaining == 0) {
					OnResourcesChanged((ResourceType)Enum.Parse(typeof(ResourceType),i.ToString()), -1);
					CmdResourceChange (indexStolenBy, i, 1);
					CmdAnnounce (l[indexStolenBy].playerName +" has stolen 1 " + ((ResourceType)Enum.Parse(typeof(ResourceType),i.ToString())).ToString() 
						+ " from " + playerName +". ");
				}
				indexRemaining--;
			}
		}
		if (!isHisTurn) {
			CmdResponseToServer ();
		}
	}

	[TargetRpc]
	public void TargetRequestChooseResource(NetworkConnection c){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		board.panelSelection.GetComponent<SelectionPanel> ().RequestResourceSelection ();
	}

	public void EndTurn(){
		GameBoard board = GameObject.Find ("GameBoard").gameObject.GetComponent<GameBoard> ();
		board.BuildVertexRequestEnd ();
		board.EnableAllKnightsAndVillagesSelection (false);
		board.panelActions.SetActive (false);
		board.panelInfoBar.SetActive (false);
		board.panelInfo.transform.GetChild (0).GetChild (1).gameObject.GetComponent<Button> ().onClick.Invoke ();
		for (int i = 0; i < 3; i++) {
			GameObject.Find ("panelFlipChart").transform.GetChild (i).gameObject.GetComponent<Button> ().interactable = false;
		}
		merchantFleet = false;
		foreach (EdgeUnit eu in roads) {
			eu.isBuiltThisRound = false;
		}
		CmdEndTurn ();
	}

	[Command]
	public void CmdSetReady(bool ready){
		isReady = ready;
	}

	[Command]
	public void CmdChangeNumOfCityWall(int change){
		numOfCityWall += change;
	}

	[Command]
	public void CmdVpChange(int change){
		vP += change;
		if (vP >= GameObject.Find ("GameServer").GetComponent<Game> ().VPsToWin) {
			GameObject.Find ("GameServer").GetComponent<Game> ().EndOfGame (GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players.IndexOf(this));
		}
	}

	[Command]
	public void CmdResourceSumChange(int change){
		resourceSum += change;
	}

	[Command]
	public void CmdProgressCardSumChange(int change){
		progressCardSum += change;
	}

	[Command]
	public void CmdDefendCountChange(int change){
		defendCount += change;
		vP += change;
	}

	[Command]
	public void CmdLongestRoadChange(int change){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		int p = l.IndexOf(this);
		int currentLongestPlayer = 0;
		int currentLongestRoad = 0;
		for(int i =0; i< l.Count;i++) {
			if (l [i].longestRoad>currentLongestRoad) {
				currentLongestPlayer = i;
				currentLongestRoad = l [i].longestRoad;
			}
		}
		if (longestRoad + 1 > currentLongestRoad) {
			if (longestRoad > 4) {
				l [currentLongestPlayer].CmdVpChange (-2);
				CmdVpChange (2);
				RpcAnnounce (l [p].playerName + " gains the Longest Road VP .");
			} else if (longestRoad == 4) {
				CmdVpChange (2);
				RpcAnnounce (l [p].playerName + " gains the Longest Road VP .");
			}
		}
		longestRoad += change;
	}

	[Command]
	public void CmdflipchartUpgrade(int type)   // 0: trade, 1:politicss, 2:science
	{
		if(type == 0)
		{	

			fcTradeLvl ++;
			if (fcTradeLvl == 4) {
				for (int i = 5; i < 8; i++) {
					TargetTradeRatioChange (this.connectionToClient,i,2);
				}
			} else if (fcTradeLvl == 5) {
				if (!GameObject.Find ("GameServer").GetComponent<Game> ().hasAchievedTrade) {
					TargetUpgradeCityToMetropole(this.connectionToClient, type);
					GameObject.Find ("GameServer").GetComponent<Game> ().hasAchievedTrade = true;
				}
			} else if (fcTradeLvl == 6) {
				List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
				bool isTheFirstOne = true;
				foreach (Player p in l) {
					if (p!=this && p.fcTradeLvl >= 6) {
						isTheFirstOne = false;
						break;
					}
				}
				if (isTheFirstOne) {
					foreach (Player p in l) {
						if (p != this) {
							foreach (Village v in p.villages) {
								if (v.vt == VillageType.TradeMetropole) {
									int vertexIndex = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions.IndexOf (v.transform.parent.gameObject);
									RpcDowngradeMetropole (vertexIndex, l.IndexOf (p));
									RpcAnnounce (this.playerName + " took " + p.playerName + "'s metropole.");
									TargetUpgradeCityToMetropole (this.connectionToClient, type);
									break;
								}
							}
						}
					}
				}
			}
		}else if (type == 1)
		{
			fcPoliticsLvl ++;
			if (fcPoliticsLvl == 5) {
				if (!GameObject.Find ("GameServer").GetComponent<Game> ().hasAchievedPolitics) {
					TargetUpgradeCityToMetropole(this.connectionToClient, type);
					GameObject.Find ("GameServer").GetComponent<Game> ().hasAchievedPolitics = true;
				}
			} else if (fcPoliticsLvl == 6) {
				List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
				bool isTheFirstOne = true;
				foreach (Player p in l) {
					if (p!=this && p.fcPoliticsLvl >= 6) {
						isTheFirstOne = false;
						break;
					}
				}
				if (isTheFirstOne) {
					foreach (Player p in l) {
						if (p != this) {
							foreach (Village v in p.villages) {
								if (v.vt == VillageType.PoliticsMetropole) {
									int vertexIndex = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions.IndexOf (v.transform.parent.gameObject);
									RpcDowngradeMetropole (vertexIndex, l.IndexOf (p));
									RpcAnnounce (this.playerName + " took " + p.playerName + "'s metropole.");
									TargetUpgradeCityToMetropole (this.connectionToClient, type);
									break;
								}
							}
						}
					}
				}
			}
		}else if(type == 2)
		{
			fcScienceLvl ++;
			if (fcScienceLvl == 5) {
				if (!GameObject.Find ("GameServer").GetComponent<Game> ().hasAchievedScience) {
					TargetUpgradeCityToMetropole(this.connectionToClient, type);
					GameObject.Find ("GameServer").GetComponent<Game> ().hasAchievedScience = true;
				}
			} else if (fcScienceLvl == 6) {
				List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
				bool isTheFirstOne = true;
				foreach (Player p in l) {
					if (p!=this && p.fcScienceLvl >= 6) {
						isTheFirstOne = false;
						break;
					}
				}
				if (isTheFirstOne) {
					foreach (Player p in l) {
						if (p != this) {
							foreach (Village v in p.villages) {
								if (v.vt == VillageType.ScienceMetropole) {
									int vertexIndex = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions.IndexOf (v.transform.parent.gameObject);
									RpcDowngradeMetropole (vertexIndex, l.IndexOf (p));
									RpcAnnounce (this.playerName + " took " + p.playerName + "'s metropole.");
									TargetUpgradeCityToMetropole (this.connectionToClient, type);
									break;
								}
							}
						}
					}
				}
			}
		}
	}

	[Command]
	public void CmdEndTurn(){
		GameObject.Find ("GameServer").GetComponent<Game> ().OnTurnEnd ();
		RpcTurnEnded ();
	}
	[Command]
	public void CmdResponseToServer(){
		GameObject.Find ("GameServer").GetComponent<Game> ().isWaiting = false;
	}

	[ClientRpc]
	public void RpcTurnEnded(){
		GameObject.Find ("GameBoard").GetComponent<GameBoard> ().panelTrade.SetActive (false);
	}
	[ClientRpc]
	public void RpcBarbarianPositionUpdate(int i){
		GameObject.Find ("panelBarbarians").transform.GetChild (2).gameObject.GetComponent<Text>().text = i.ToString();
	}

	[ClientRpc]
	public void RpcBarbarianStrengthUpdate(int newStrength, bool danger){
		GameObject.Find ("panelBarbarians").transform.GetChild (0).gameObject.GetComponent<Text>().text = newStrength.ToString();
		Color toColor;
		if (danger) {
			toColor = Color.red;
		} else {
			toColor = Color.green;
		}
		for (int i = 0; i < 3; i++){
			GameObject.Find ("panelBarbarians").transform.GetChild (i).gameObject.GetComponent<Text>().color = toColor;
		}
	}

	[ClientRpc]
	public void RpcKnightStrengthUpdate(int newStrength, bool danger){
		GameObject.Find ("panelBarbarians").transform.GetChild (1).gameObject.GetComponent<Text>().text = newStrength.ToString();
		Color toColor;
		if (danger) {
			toColor = Color.red;
		} else {
			toColor = Color.green;
		}
		for (int i = 0; i < 3; i++){
			GameObject.Find ("panelBarbarians").transform.GetChild (i).gameObject.GetComponent<Text>().color = toColor;
		}
	}

	[TargetRpc]
	public void TargetRequestBuildSettlementAndRoad(NetworkConnection c){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		board.RequestBuildSettlementRoundOne();
	}

	[TargetRpc]
	public void TargetRequestBuildCityAndRoad(NetworkConnection c){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		board.RequestBuildCity();
	}

	[TargetRpc]
	public void TargetGiveTurn(NetworkConnection c){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		board.GetTurn();
	}

	[TargetRpc]
	public void TargetResourceChange(NetworkConnection c, int rsType, int number){
		OnResourcesChanged ((ResourceType)Enum.Parse(typeof(ResourceType),rsType.ToString()), number);
	}


	[Command]
	public void CmdBuildSettlement(int index, bool isSetUp){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList>()._players;
		int p = l.IndexOf(this);
		RpcBuildSettlement (index, p, isSetUp);
	}

	[ClientRpc]
	public void RpcBuildSettlement(int vertexIndex, int playerIndex, bool isSetUp){
		GameObject junction = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions [vertexIndex];
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList>()._players;
		GameObject settlement = Instantiate (GameObject.Find ("GameBoard").GetComponent<GameBoard>().villageObj, junction.transform.position, Quaternion.identity);
		settlement.transform.GetChild (0).gameObject.SetActive(true);
		settlement.transform.SetParent (junction.transform);

		junction.GetComponent<Vertex> ().isBuildable = false;
		junction.GetComponent<Vertex> ().isOccupied = true;
		junction.GetComponent<Vertex> ().isRealOccupied = true;

		// color it
		if (l[playerIndex].playerColor == Color.blue)
		{
			settlement.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/Settlement_Blue");
		}
		if (l[playerIndex].playerColor == Color.gray)
		{
			settlement.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/Settlement_Gray");
		}
		if (l[playerIndex].playerColor == Color.green)
		{
			settlement.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/Settlement_Green");
		}
		if (l[playerIndex].playerColor == Color.red)
		{
			settlement.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/Settlement_Red");
		}


		for (int j = 0; j < 3; j++)
		{
			if (junction.GetComponent<Vertex> ().adjacentEdges [j] != null) {
				junction.GetComponent<Vertex> ().adjacentEdges [j].adjacentVertices [0].isBuildable = false;
				junction.GetComponent<Vertex> ().adjacentEdges [j].adjacentVertices [0].isOccupied = true;
				junction.GetComponent<Vertex> ().adjacentEdges [j].adjacentVertices [1].isBuildable = false;
				junction.GetComponent<Vertex> ().adjacentEdges [j].adjacentVertices [1].isOccupied = true;
			}
		}

		HarbourType type = junction.GetComponent<Vertex> ().harbourtype;
		// local 
		if (l [playerIndex].isLocalPlayer) {
			if (!isSetUp) {
				OnResourcesChanged (ResourceType.Lumber, -1);
				OnResourcesChanged (ResourceType.Brick, -1);
				OnResourcesChanged (ResourceType.Wool, -1);
				OnResourcesChanged (ResourceType.Grain, -1);
			}

			if (type == HarbourType.Generic) {
				for (int i = 0; i < 8; i++) {
					if (l[playerIndex].tradeRatios[i] > 3) {
						l[playerIndex].tradeRatios[i] = 3;
					}
				}
			} else if (type == HarbourType.Lumber) {
				l [playerIndex].tradeRatios [0] = 2;
			} else if (type == HarbourType.Brick) {
				l [playerIndex].tradeRatios [1] = 2;
			} else if (type == HarbourType.Wool) {
				l [playerIndex].tradeRatios [2] = 2;
			} else if (type == HarbourType.Grain) {
				l [playerIndex].tradeRatios [3] = 2;
			} else if (type == HarbourType.Ore) {
				l [playerIndex].tradeRatios [4] = 2;
			}

			l [playerIndex].CmdVpChange (1);
		}

		settlement.GetComponent<Village> ().owner = l[playerIndex];
		settlement.GetComponent<Village> ().vt = VillageType.Settlement;
		settlement.GetComponent<Village> ().hasCityWall = false;
		l [playerIndex].villages.Add (settlement.GetComponent<Village>());
		if (!isSetUp && l [playerIndex].isLocalPlayer) {
			GameObject.Find ("GameBoard").GetComponent<GameBoard> ().EnableAllKnightsAndVillagesSelection (true);
		}
	}

	[Command]
	public void CmdBuildRoad(int index)
	{
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		int p = l.IndexOf(this);
		bool isSetUp = false;
		if (GameObject.Find ("GameServer").GetComponent<Game> ().currentPhase == GamePhase.SetupRoundOne ||
		   GameObject.Find ("GameServer").GetComponent<Game> ().currentPhase == GamePhase.SetupRoundTwo) {
			isSetUp = true;
		}
		RpcBuildRoad(index, p,isSetUp);
	}

	[ClientRpc]
	public void RpcBuildRoad(int edgeIndex, int playerIndex, bool isSetUp)
	{
		GameObject e = GameObject.Find("GameBoard").GetComponent<GameBoard>().edges[edgeIndex];
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		GameObject eunit = Instantiate(GameObject.Find("GameBoard").GetComponent<GameBoard>().roadObj, e.transform.position, e.transform.rotation);
		eunit.transform.GetChild(0).gameObject.SetActive(true);
		eunit.transform.SetParent(e.transform);
		//eunit.transform.parent.GetChild(0).gameObject.SetActive(false);
		//eunit.transform.parent.gameObject.SetActive(true);
		e.GetComponent<Edge> ().isOccupied = true;
		e.GetComponent<Edge> ().isShipBuildable = false;
		e.GetComponent<Edge> ().isBuildable = false;
		e.GetComponent<Edge> ().myEdgeUnit = eunit.GetComponent<EdgeUnit> ();

		// color it
		if (l[playerIndex].playerColor == Color.blue)
		{
			eunit.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/_RoadBlue");
		}
		if (l[playerIndex].playerColor == Color.gray)
		{
			eunit.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/_RoadGray");
		}
		if (l[playerIndex].playerColor == Color.green)
		{
			eunit.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/_RoadGreen");
		}
		if (l[playerIndex].playerColor == Color.red)
		{
			eunit.transform.GetChild(0).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/_RoadRed");
		}

		// local stuff
		if (l [playerIndex].isLocalPlayer) {
			if (!isSetUp) {
				if (roadBuilding > 0) {
					roadBuilding--;
				} else {
					OnResourcesChanged (ResourceType.Lumber, -1);
					OnResourcesChanged (ResourceType.Brick, -1);
				}
			}

			for (int i = 0; i < 2; i++) {
				if (e.GetComponent<Edge> ().adjacentVertices [i].isOccupied == false) {
					e.GetComponent<Edge> ().adjacentVertices [i].isBuildable = true;
				}
				if (e.GetComponent<Edge> ().adjacentVertices [i].isRealOccupied == false) {
					for (int j = 0; j < 3; j++) {
						if (e.GetComponent<Edge> ().adjacentVertices [i].adjacentEdges [j] != null) {
							if(e.GetComponent<Edge> ().adjacentVertices [i].adjacentEdges [j].isOccupied == false &&
								e.GetComponent<Edge> ().adjacentVertices [i].adjacentEdges [j].isOccupiedByShip == false){
								if (e.GetComponent<Edge> ().adjacentVertices [i].adjacentEdges [j].isLand) {
									e.GetComponent<Edge> ().adjacentVertices [i].adjacentEdges [j].isBuildable = true;
								} 
							}

						}
					}
				}

			}

			// Update Longest Road
			CheckLongestRoad(e.GetComponent<Edge>());
		}

		// set attributes of road
		eunit.GetComponent<EdgeUnit>().owner = l[playerIndex];
		eunit.GetComponent<EdgeUnit>().ifship = false;
		l [playerIndex].roads.Add (eunit.GetComponent<EdgeUnit>());
	}

	[Command]
	public void CmdBuildShip(int index){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		int p = l.IndexOf(this);
		RpcBuildShip(index, p);
	}

	[ClientRpc]
	public void RpcBuildShip(int edgeIndex, int playerIndex){
		GameObject e = GameObject.Find("GameBoard").GetComponent<GameBoard>().edges[edgeIndex];
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		GameObject eunit = Instantiate(GameObject.Find("GameBoard").GetComponent<GameBoard>().roadObj, e.transform.position, e.transform.rotation);

		eunit.transform.GetChild(1).gameObject.SetActive(true);
		eunit.transform.SetParent(e.transform);
		eunit.GetComponent<EdgeUnit> ().isClose = false;
		e.GetComponent<Edge> ().isOccupiedByShip = true;
		e.GetComponent<Edge> ().isBuildable = false;
		e.GetComponent<Edge> ().isShipBuildable = false;
		e.GetComponent<Edge> ().myEdgeUnit = eunit.GetComponent<EdgeUnit> ();

		// color it
		if (l[playerIndex].playerColor == Color.blue)
		{
			eunit.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/Ship_blue");
		}
		if (l[playerIndex].playerColor == Color.gray)
		{
			eunit.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/Ship_gray");
		}
		if (l[playerIndex].playerColor == Color.green)
		{
			eunit.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/Ship_green");
		}
		if (l[playerIndex].playerColor == Color.red)
		{
			eunit.transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/Ship_red");
		}

		if (l [playerIndex].isLocalPlayer) {
			OnResourcesChanged (ResourceType.Lumber, -1);
			OnResourcesChanged (ResourceType.Wool, -1);

			for (int i = 0; i < 2; i++) {
				if (e.GetComponent<Edge> ().adjacentVertices [i].isOccupied == false || e.GetComponent<Edge> ().adjacentVertices [i].isLand) {
					e.GetComponent<Edge> ().adjacentVertices [i].isBuildable = true;
				}


				if (e.GetComponent<Edge> ().adjacentVertices [i].isRealOccupied) {
					if (e.GetComponent<Edge> ().adjacentVertices [i].transform.childCount > 1 && (e.GetComponent<Edge> ().adjacentVertices [i].transform.GetChild (1).gameObject.GetComponent<Village> () != null)) {
						if (e.GetComponent<Edge> ().adjacentVertices [i].transform.GetChild (1).gameObject.GetComponent<Village> ().owner.isLocalPlayer) {
							int otherVertex = 0;
							if (i == 0) {
								otherVertex = 1;
							}
							foreach (Edge adjEdge in e.GetComponent<Edge> ().adjacentVertices [otherVertex].adjacentEdges) {
								if (adjEdge.isOccupiedByShip && adjEdge != e.GetComponent<Edge> () && adjEdge.myEdgeUnit.owner.isLocalPlayer) {
									eunit.GetComponent<EdgeUnit> ().isClose = true;
								}
							}
						}
					}
				}


				if (e.GetComponent<Edge> ().adjacentVertices [i].isRealOccupied == false) {
					for (int j = 0; j < 3; j++) {
						if (e.GetComponent<Edge> ().adjacentVertices [i].adjacentEdges [j] != null) {
							if(e.GetComponent<Edge> ().adjacentVertices [i].adjacentEdges [j].isOccupied == false &&
								e.GetComponent<Edge> ().adjacentVertices [i].adjacentEdges [j].isOccupiedByShip == false){
								if (e.GetComponent<Edge> ().adjacentVertices [i].adjacentEdges [j].isCoast) {
									e.GetComponent<Edge> ().adjacentVertices [i].adjacentEdges [j].isShipBuildable = true;
								}
							}

						}
					}
				}

			}

			// Update Longest Road
			CheckLongestRoad(e.GetComponent<Edge>());
		}

		// set attributes of ship - THIS IS THE PART I HAVENT DONE
		eunit.GetComponent<EdgeUnit>().owner = l[playerIndex];
		eunit.GetComponent<EdgeUnit>().ifship = true;
		l [playerIndex].roads.Add(eunit.GetComponent<EdgeUnit>());
		eunit.GetComponent<EdgeUnit> ().isBuiltThisRound = true;
		//eunit.GetComponent<EdgeUnit>().coastalRoute.Add(eunit.GetComponent<Ship>);

	}

	void CheckLongestRoad(Edge e)
	{
		//		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		//		int player = l.IndexOf (this);
		//
		// Get adjacent edge list without current edge
		Vertex adjVertex0 = e.adjacentVertices[0];
		List<Edge> adjEdge0Buffer = new List<Edge>();
		for (int i = 0; i < 3; i++)
		{
			if (adjVertex0.adjacentEdges[i] != e)
			{
				adjEdge0Buffer.Add(adjVertex0.adjacentEdges[i]);
			}
		}
		Vertex adjVertex1 = e.adjacentVertices[1];
		List<Edge> adjEdge1Buffer = new List<Edge>();
		for (int i = 0; i < 3; i++)
		{
			if (adjVertex1.adjacentEdges[i] != e)
			{
				adjEdge1Buffer.Add(adjVertex1.adjacentEdges[i]);
			}
		}
		int sum = 1;
		int maxbuffer0 = HelpCheckRoad(adjEdge0Buffer[0], adjVertex0, e.myEdgeUnit.ifship);
		int maxbuffer1 = HelpCheckRoad(adjEdge1Buffer[0], adjVertex1, e.myEdgeUnit.ifship);
		if (adjEdge0Buffer[1] != null)
		{
			maxbuffer0 = Math.Max(maxbuffer0, HelpCheckRoad(adjEdge0Buffer[1], adjVertex0, e.myEdgeUnit.ifship));
		}
		if (adjEdge1Buffer[1] != null)
		{
			maxbuffer1 = Math.Max(maxbuffer1, HelpCheckRoad(adjEdge1Buffer[1], adjVertex1, e.myEdgeUnit.ifship));
		}
		sum = sum + maxbuffer0 + maxbuffer1;


		if (sum > longestRoad)
		{
			int dif = sum - longestRoad;
			for (int i = 0; i < dif; i++)
			{
				CmdLongestRoadChange(1);
			}
		}

	}

	// Support method for CheckLongestRoad
	int HelpCheckRoad(Edge e, Vertex v, bool ifShip)
	{
		if (e.isOccupied)
		{
			bool ifVillageOwned = false;
			if (v.transform.childCount > 1) {
				if (v.transform.GetChild (1).GetComponent<Village> () != null) {
					Village vlg = v.transform.GetChild (1).GetComponent<Village> ();
					if (vlg.owner == this) {
						ifVillageOwned = true;
					}
				}
			}

			if (e.myEdgeUnit.owner == this && (ifShip == e.myEdgeUnit.ifship|| ifVillageOwned == true))
			{
				List<Vertex> vBuffer = new List<Vertex>();
				for (int i = 0; i < 2; i++)
				{
					if (e.adjacentVertices[i] != v)
					{
						vBuffer.Add(e.adjacentVertices[i]);
					}
				}
				List<Edge> adjEdgeBuffer = new List<Edge>();
				for (int i = 0; i < 3; i++)
				{
					if (vBuffer[0].adjacentEdges[i] != e && vBuffer[0].adjacentEdges[i] != null)
					{
						adjEdgeBuffer.Add(vBuffer[0].adjacentEdges[i]);
					}
				}
				int maxbuffer = HelpCheckRoad(adjEdgeBuffer[0], vBuffer[0],e.myEdgeUnit.ifship);
				if (adjEdgeBuffer.Count > 1)
				{
					maxbuffer = Math.Max(maxbuffer, HelpCheckRoad(adjEdgeBuffer[1], vBuffer[0],e.myEdgeUnit.ifship));
				}

				return 1 + maxbuffer;

			}
		}
		return 0;
	}



	[Command]
	public void CmdUpgradeSettlement(int index){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		int p = l.IndexOf(this);
		bool isSetUp = false;
		if (GameObject.Find ("GameServer").GetComponent<Game> ().currentPhase == GamePhase.SetupRoundOne ||
			GameObject.Find ("GameServer").GetComponent<Game> ().currentPhase == GamePhase.SetupRoundTwo) {
			isSetUp = true;
		}
		RpcUpgradeSettlement (index, p, isSetUp);
		GameObject.Find ("GameServer").GetComponent<Game> ().OnBarbarianStrengthChanged (1);

	}

	[ClientRpc]
	public void RpcUpgradeSettlement(int vertexIndex, int playerIndex, bool isSetUp){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		GameObject junction = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions [vertexIndex];
		GameObject village = junction.transform.GetChild (1).gameObject;

		village.transform.GetChild (0).gameObject.SetActive (false);
		village.transform.GetChild (1).gameObject.SetActive (true);

		// color it
		if (l[playerIndex].playerColor == Color.blue)
		{
			village.transform.GetChild(1).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/City_blue");
		}
		if (l[playerIndex].playerColor == Color.gray)
		{
			village.transform.GetChild(1).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/City_gray");
		}
		if (l[playerIndex].playerColor == Color.green)
		{
			village.transform.GetChild(1).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/City_green");
		}
		if (l[playerIndex].playerColor == Color.red)
		{
			village.transform.GetChild(1).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/City_red");
		}

		// set attributes
		village.GetComponent<Village>().vt = VillageType.City;

		if (l [playerIndex].isLocalPlayer) {
			if (!isSetUp) {
				if (isMedicine) {
					OnResourcesChanged (ResourceType.Grain, -1);
					OnResourcesChanged (ResourceType.Ore, -2);
					isMedicine = false;
				} else {
					OnResourcesChanged (ResourceType.Grain, -2);
					OnResourcesChanged (ResourceType.Ore, -3);
				}
			}


			l [playerIndex].CmdVpChange (1);
			if (!isSetUp) {
				GameObject.Find ("GameBoard").GetComponent<GameBoard> ().RenewImprovementsAvailability();
			}
		}
	}  

	[TargetRpc]
	public void TargetRequestDowngradeCity(NetworkConnection target){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		board.panelInfo.GetComponent<InfoPanel> ().pushMessage ("Barbarians raided one of your cities. You have to choose a city to be downgraded.",board.RequestDowngradeCity, board.RequestDowngradeCity);
	}

	[Command]
	public void CmdDowngradeCity(int index){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		int p = l.IndexOf(this);
		RpcDowngradeCity(index, p);
		GameObject.Find ("GameServer").GetComponent<Game> ().OnBarbarianStrengthChanged (-1);
	}
	[ClientRpc]
	public void RpcDowngradeCity(int vertexIndex, int playerIndex){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		GameObject junction = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions [vertexIndex];
		GameObject village = junction.transform.GetChild (1).gameObject;

		village.transform.GetChild (1).gameObject.SetActive (false); //metropole to false
		village.transform.GetChild (0).gameObject.SetActive (true);
		if (l [playerIndex].isLocalPlayer) {
			l [playerIndex].CmdVpChange (-1);
		}
		// set attributes
		village.GetComponent<Village>().vt = VillageType.Settlement;
		if (village.GetComponent<Village> ().hasCityWall) {
			village.GetComponent<Village> ().hasCityWall = false;
			if (l [playerIndex].isLocalPlayer) {
				l [playerIndex].CmdChangeNumOfCityWall (-1);
			}
			village.transform.GetChild (3).gameObject.SetActive (false); 
		}
	}

	[TargetRpc]
	public void TargetUpgradeCityToMetropole(NetworkConnection target, int type){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		board.panelInfo.GetComponent<InfoPanel> ().pushMessage (
			"You can now build a metropole of " + ((Improvement)Enum.Parse(typeof(Improvement),type.ToString())).ToString()
			+ "! Select a city to build a metropole!",
			delegate{
				board.RequestBuildMetropole (type);
			},
			delegate{
				board.RequestBuildMetropole (type);
			}
		);

	}

	[Command]
	public void CmdUpgradeCityToMetropole(int vertexIndex, int type){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		int p = l.IndexOf(this);
		RpcUpgradeCityToMetropole(vertexIndex, p, type);
	}

	[ClientRpc]
	public void RpcUpgradeCityToMetropole(int vertexIndex, int playerIndex, int type){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		GameObject junction = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions [vertexIndex];
		GameObject village = junction.transform.GetChild (1).gameObject;

		village.transform.GetChild (1).gameObject.SetActive (false);
		village.transform.GetChild (2).gameObject.SetActive (true);

		if (l[playerIndex].playerColor == Color.blue)
		{
			village.transform.GetChild(2).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/Metro_blue");
		}
		if (l[playerIndex].playerColor == Color.gray)
		{
			village.transform.GetChild(2).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/Metro_gray");
		}
		if (l[playerIndex].playerColor == Color.green)
		{
			village.transform.GetChild(2).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/Metro_green");
		}
		if (l[playerIndex].playerColor == Color.red)
		{
			village.transform.GetChild(2).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/Metro_red");
		}


		if (type == 0) {
			village.GetComponent<Village> ().vt = VillageType.TradeMetropole;
		}
		else if (type == 1) {
			village.GetComponent<Village> ().vt = VillageType.PoliticsMetropole;
		}
		else if (type == 2){
			village.GetComponent<Village> ().vt = VillageType.ScienceMetropole;
		}

		if (l [playerIndex].isLocalPlayer) {
			l [playerIndex].CmdVpChange (2); 		//increase vp points
			GameObject.Find ("GameBoard").GetComponent<GameBoard> ().RenewImprovementsAvailability();
		}
	}

	[ClientRpc]
	public void RpcDowngradeMetropole(int vertexIndex, int playerIndex){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		GameObject junction = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions [vertexIndex];
		GameObject village = junction.transform.GetChild (1).gameObject;

		village.transform.GetChild (2).gameObject.SetActive (false); //metropole to false
		village.transform.GetChild (1).gameObject.SetActive (true);

		if (l [playerIndex].isLocalPlayer) {
			l [playerIndex].CmdVpChange (-2); //reduce vp points
		}
		// set attributes
		village.GetComponent<Village>().vt = VillageType.City;
	}

	[Command]
	public void CmdBuildCityWall(int index){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		int p = l.IndexOf(this);
		RpcBuildCityWall (index, p);
	}

	[ClientRpc]
	public void RpcBuildCityWall(int vertexIndex, int playerIndex){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		GameObject junction = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions [vertexIndex];
		GameObject village = junction.transform.GetChild (1).gameObject;

		village.transform.GetChild (3).gameObject.SetActive (true);

		// color it
		if (l[playerIndex].playerColor == Color.blue)
		{
			village.transform.GetChild(3).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/CityWall_blue");
		}
		if (l[playerIndex].playerColor == Color.gray)
		{
			village.transform.GetChild(3).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/CityWall_gray");
		}
		if (l[playerIndex].playerColor == Color.green)
		{
			village.transform.GetChild(3).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/CityWall_green");
		}
		if (l[playerIndex].playerColor == Color.red)
		{
			village.transform.GetChild(3).gameObject.GetComponent<Renderer>().sharedMaterial = Resources.Load<Material>("Materials/CityWall_red");
		}

		// set attributes
		village.GetComponent<Village>().hasCityWall = true;
		if (l [playerIndex].isLocalPlayer) {
			if (!isEnginner) {
				OnResourcesChanged (ResourceType.Brick, -2);
			} else {
				isEnginner = false;
			}


			CmdChangeNumOfCityWall (1);

		}

	}  

	[Command]
	public void CmdPurchaseKnight(int index){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList>()._players;
		int p = l.IndexOf(this);
		RpcPurchaseKnight (index, p);
	}

	[ClientRpc]
	public void RpcPurchaseKnight(int vertexIndex, int playerIndex){
		GameObject junction = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions [vertexIndex];
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList>()._players;
		GameObject knight = Instantiate (GameObject.Find ("GameBoard").GetComponent<GameBoard>().knightObj, junction.transform.position, Quaternion.identity);
		knight.transform.GetChild (0).gameObject.SetActive(true);
		knight.transform.SetParent (junction.transform);

		junction.GetComponent<Vertex> ().isBuildable = false;
		junction.GetComponent<Vertex> ().isOccupied = true;

		// color it
		Material toBeRendered = Resources.Load<Material>("Materials/WK_StandardUnits_generic");
		Material toBeRenderedHorse = Resources.Load<Material>("Materials/WK_Horse_A");
		if (l[playerIndex].playerColor == Color.blue)
		{
			toBeRendered = Resources.Load<Material>("Materials/WK_StandardUnits_Blue");
			toBeRenderedHorse = Resources.Load<Material>("Materials/WK_Horse_C");
		}
		if (l[playerIndex].playerColor == Color.gray)
		{
			toBeRendered = Resources.Load<Material>("Materials/WK_StandardUnits_White");
			toBeRenderedHorse = Resources.Load<Material>("Materials/WK_Horse_D");
		}
		if (l[playerIndex].playerColor == Color.green)
		{
			toBeRendered = Resources.Load<Material>("Materials/WK_StandardUnits_Green");
			toBeRenderedHorse = Resources.Load<Material>("Materials/WK_Horse_A");
		}
		if (l[playerIndex].playerColor == Color.red)
		{
			toBeRendered = Resources.Load<Material>("Materials/WK_StandardUnits_Red");
			toBeRenderedHorse = Resources.Load<Material>("Materials/WK_Horse_B");
		}
		for (int i = 0; i < 3; i++) {
			knight.transform.GetChild(i).GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial = toBeRendered;
		}
		knight.transform.GetChild(2).GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterial = toBeRenderedHorse;

		junction.GetComponent<Vertex> ().isRealOccupied = true;
		knight.GetComponent<Knight> ().owner = l[playerIndex];
		knight.GetComponent<Knight> ().level = 1;
		knight.GetComponent<Knight> ().isActive = false;
		l [playerIndex].knights.Add (knight.GetComponent<Knight>());
		if (l [playerIndex].isLocalPlayer) {
			OnResourcesChanged (ResourceType.Wool, -1);
			OnResourcesChanged (ResourceType.Ore, -1);

			GameObject.Find ("GameBoard").GetComponent<GameBoard> ().EnableAllKnightsAndVillagesSelection (true);
		}
	}

	[Command]
	public void CmdSetKnightActive(int vertexIndex, bool activated){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		int p = l.IndexOf(this);
		RpcSetKnightActive (vertexIndex, p, activated);
		GameObject junction = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions [vertexIndex];
		Knight knight = junction.transform.GetChild (1).gameObject.GetComponent<Knight>();
		if (activated) {
			GameObject.Find ("GameServer").GetComponent<Game> ().OnKnightStrengthChanged (knight.level);
		} else {
			GameObject.Find ("GameServer").GetComponent<Game> ().OnKnightStrengthChanged (-knight.level);
		}
	}
	[ClientRpc]
	public void RpcSetKnightActive(int vertexIndex, int playerIndex, bool activated){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		GameObject junction = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions [vertexIndex];
		GameObject knight = junction.transform.GetChild (1).gameObject;
		knight.GetComponent<Knight>().isActive = activated;
		knight.GetComponent<Knight>().hasMovedThisTurn = false;
		knight.transform.GetChild (knight.GetComponent<Knight>().level-1).gameObject.GetComponent<Animator> ().SetBool ("isActive", activated);
		if (l [playerIndex].isLocalPlayer) {
			if (isWarLord > 0) {
				isWarLord--;
			}else{
				OnResourcesChanged (ResourceType.Grain, -1);
			}
		}
	}

	[Command]
	public void CmdUpgradeKnight(int index, bool activated){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		int p = l.IndexOf(this);
		RpcUpgradeKnight (index, p, activated);
		if (activated) {
			GameObject.Find ("GameServer").GetComponent<Game> ().OnKnightStrengthChanged (1);
		}
	}

	[ClientRpc]
	public void RpcUpgradeKnight(int vertexIndex, int playerIndex, bool activated){
		//A knight may be promoted on the same turn it was built, and it is not inactive or active
		//it cannot be promoted twice the same turn. 
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		GameObject junction = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions [vertexIndex];
		GameObject knight = junction.transform.GetChild (1).gameObject;

		knight.transform.GetChild (knight.GetComponent<Knight> ().level - 1).gameObject.SetActive (false);
		knight.transform.GetChild (knight.GetComponent<Knight> ().level).gameObject.SetActive (true);
		if (activated) {
			knight.transform.GetChild (knight.GetComponent<Knight>().level).gameObject.GetComponent<Animator> ().SetBool ("isActive", activated);
		}
		knight.GetComponent<Knight> ().level++;
		knight.GetComponent<Knight> ().hasPromotedThisTurn = true;

		if (l [playerIndex].isLocalPlayer) {
			if (isSmith > 0) {
				isSmith--;
			}else{
				OnResourcesChanged (ResourceType.Wool, -1);
				OnResourcesChanged (ResourceType.Ore, -1);
			}
		}

	}

	[TargetRpc]
	public void TargetAskPlayAlchemistCard(NetworkConnection c){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		board.panelInfo.GetComponent<InfoPanel> ().pushMessage ("Do you want to play the Alchemist card?",
		delegate {
				board.panelProgressCard.GetComponent<ProgressCardList>().playCardOfType(0);
		},
		delegate {
				CmdResponseToServer();
		});
	}

	[Command]
	public void CmdSetDice(int whiteDice, int redDice){
		Game server = GameObject.Find ("GameServer").GetComponent<Game> ();
		server.yellowDie = whiteDice;
		server.redDie = redDice;
	}

	[TargetRpc] 
	public void TargetRequestMoveRobber(NetworkConnection c){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		board.panelInfo.GetComponent<InfoPanel> ().pushMessage ("Which do you want to move, The robber or the pirate? Confirm to move robber, reject to move pirate.",board.RequestMoveRobber,board.RequestMovePirate);
	}

	[Command]
	public void CmdMoveRobber(int index){
		RpcMoveRobber (index);
	}
	[ClientRpc] 
	public void RpcMoveRobber(int hexIndex){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		GameObject targetHex = board.tiles [hexIndex];
		board.robber.transform.position = targetHex.transform.position;
		board.robber.transform.SetParent (targetHex.transform);
	}

	[Command]
	public void CmdMovePirate(int index){
		RpcMovePirate (index);
	}
	[ClientRpc] 
	public void RpcMovePirate(int hexIndex){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		GameObject targetHex = board.tiles [hexIndex];
		board.pirate.transform.position = targetHex.transform.position;
		board.pirate.transform.SetParent (targetHex.transform);
	}

	[Command]
	public void CmdMoveMerchant(int index){
		foreach (Player p in GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players) {
			if (p.hasMerchant) {
				p.vP--;
				p.hasMerchant = false;
			}
		}
		RpcMoveMerchant (index);
		vP++;
		hasMerchant = true;
	}

	[ClientRpc] 
	public void RpcMoveMerchant(int hexIndex){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		GameObject targetHex = board.tiles [hexIndex];
		board.merchant.transform.position = targetHex.transform.position;
		board.merchant.transform.SetParent (targetHex.transform);
	}

	// basically just change the panel layout

	[TargetRpc]
	public void TargetGetProgressCard(NetworkConnection c, int category, int cardType){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		board.panelProgressCard.GetComponent<ProgressCardList> ().Add (category, cardType, false);
	}

	[TargetRpc]
	public void TargetRequestDrawProgressCard(NetworkConnection c, bool canTrade, bool canPolitics, bool canScience){
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
		board.panelSelection.GetComponent<SelectionPanel> ().RequestImprovementSelection (canTrade, canPolitics, canScience);
	}

	[Command]
	public void CmdDiscardProgressCard(int category, int cardType){
		Game server = GameObject.Find ("GameServer").GetComponent<Game> ();
		ProgressCard newCard = new ProgressCard ();
		newCard = newCard.setPg ((pgCardType)Enum.Parse (typeof(pgCardType), cardType.ToString ()), category);
		if (category == (int)Improvement.Trade) {
			server.tradeCardsQueue.Enqueue (newCard);
		} else if (category == (int)Improvement.Politics) {
			server.politicsCardsQueue.Enqueue (newCard);
		} else if (category == (int)Improvement.Science) {
			server.scienceCardsQueue.Enqueue (newCard);
		}
		progressCardSum--;
	}

	[Command]
	public void CmdRequestDrawProgressCard(int category){
		GameObject.Find ("GameServer").GetComponent<Game> ().selectionIndicator = category;
	}


	[Command]
	public void CmdPlaySpy(int ofPlayerIndex){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		l [ofPlayerIndex].TargetRequestRevealProgressCards (l[ofPlayerIndex].connectionToClient,l.IndexOf(this));
	}

	[Command]
	public void CmdRevealProgressCardToPlayer(int toPlayerIndex, int[] categories, int[] cardTypes){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		l [toPlayerIndex].TargetLookAtProgressCards (l [toPlayerIndex].connectionToClient, l.IndexOf (this), categories, cardTypes);
	}

	[Command]
	public void CmdTakeProgressCard(int fromPlayerIndex, int cardType){
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		l [fromPlayerIndex].TargetProgressCardTaken (l [fromPlayerIndex].connectionToClient, cardType);
	}
	[TargetRpc]
	public void TargetRequestRevealProgressCards(NetworkConnection c, int toPlayer){
		GameObject.Find ("GameBoard").GetComponent<GameBoard> ().panelProgressCard.GetComponent<ProgressCardList> ().RevealProgressCardsTo (toPlayer);
	}

	[TargetRpc]
	public void TargetLookAtProgressCards(NetworkConnection c, int ofPlayerIndex, int[] categories, int[] cardTypes){
		GameObject.Find ("GameBoard").GetComponent<GameBoard> ().panelProgressCard.GetComponent<ProgressCardList> ().LookAndChooseCards (
			ofPlayerIndex,categories,cardTypes
		);
	}

	[TargetRpc]
	public void TargetProgressCardTaken(NetworkConnection c, int cardType){
		GameObject.Find ("GameBoard").GetComponent<GameBoard> ().panelProgressCard.GetComponent<ProgressCardList> ().CardTaken (cardType);
	}


	[ClientRpc]
	public void RpcRollDice(int die1, int die2, int die3){
		GameObject.Find ("GameBoard").GetComponent<GameBoard> ().panelDice.SetActive (true);
		Image whiteDie = GameObject.Find ("GameBoard").GetComponent<GameBoard>().panelDice.transform.GetChild(0).GetComponent<Image>();
		Image redDie = GameObject.Find ("GameBoard").GetComponent<GameBoard>().panelDice.transform.GetChild(1).GetComponent<Image>();
		Image eventDie = GameObject.Find ("GameBoard").GetComponent<GameBoard>().panelDice.transform.GetChild(2).GetComponent<Image>();

		whiteDie.sprite = Resources.Load<Sprite> ("UI/dice/dices_white_" + die1.ToString ());
		redDie.sprite = Resources.Load<Sprite> ("UI/dice/dices_red_"+die2.ToString());

		if (die3 == 1 || die3 == 2 || die3 == 3) {
			eventDie.sprite = Resources.Load<Sprite> ("UI/dice/dices_event_pirate");
		} else if (die3 == 4) {
			eventDie.sprite = Resources.Load<Sprite> ("UI/dice/dices_event_trade");
		} else if (die3 == 5) {
			eventDie.sprite = Resources.Load<Sprite> ("UI/dice/dices_event_politics");
		} else if (die3 == 6) {
			eventDie.sprite = Resources.Load<Sprite> ("UI/dice/dices_event_science");
		}


	}
	// Three method upgrade flipchart
	[Command]
	public void CmdAnnounce(string announcement){
		RpcAnnounce (announcement);
	}
	[ClientRpc]
	public void RpcAnnounce(string announcement){
		GameObject.Find ("GameBoard").GetComponent<GameBoard> ().panelInfo.GetComponent<InfoPanel> ().pushMessage (announcement,null,null);
	}

	public void SaveData(NetworkConnection c, string fileName){
		MySaveGame mysave = new MySaveGame();
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		int playerIndex = l.IndexOf(this);
		mysave.playerName = playerName;

		if (playerColor == Color.blue) {
			mysave.playerColor = "blue";
		} else if (playerColor == Color.green) {
			mysave.playerColor = "green";
		} else if (playerColor == Color.gray) {
			mysave.playerColor = "gray";
		}else if(playerColor == Color.red){
			mysave.playerColor="red";
		}
		foreach(ProgressCard pc in pgCard) // instead of add pgcard, we save the type of it
		{
			int buffer = (int)pc.myType;
			mysave.pgCard.Add(buffer);
		}
		mysave.numOfCityWall= numOfCityWall;
		mysave.vP = vP;
		mysave.resourceSum = resourceSum;
		mysave.progressCardSum = progressCardSum;
		mysave.defendCount = defendCount;
		mysave.longestRoad = longestRoad;
		mysave.fcTradeLvl = fcTradeLvl;
		mysave.fcPoliticsLvl = fcPoliticsLvl;
		mysave.fcScienceLvl = fcScienceLvl;
		mysave.resources = resources;
		mysave.tradeRatios = tradeRatios;
		mysave.villages = villages;
		mysave.knights = knights;
		mysave.roads = roads;
		Debug.Log (resources);
		string myname = fileName + "_" + playerIndex;
		SaveGameSystem.SaveGame (mysave, myname);
	}

    public void LoadData(NetworkConnection c, string fileName)
    {
        List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
        int playerIndex = l.IndexOf(this);
        string myname = fileName + "_" + playerIndex;
        MySaveGame loadGame = SaveGameSystem.LoadGame(myname) as MySaveGame;

        CmdChangeNumOfCityWall(loadGame.numOfCityWall);
		CmdVpChange(loadGame.resourceSum);
//        OnResourceSumChanged(loadGame.resourceSum);
		CmdDefendCountChange(loadGame.defendCount);
		CmdLongestRoadChange(loadGame.longestRoad);
        for(int i = 0; i < loadGame.fcTradeLvl -1; i++)
        {
           CmdflipchartUpgrade(0);
        }
        for (int i = 0; i < loadGame.fcPoliticsLvl; i++)
        {
            CmdflipchartUpgrade(1);
        }
        for (int i = 0; i < loadGame.fcScienceLvl; i++)
        {
            CmdflipchartUpgrade(2);
        }
		Debug.Log (loadGame.resources);
        for(int i=0; i < 8; i++)
        {
            OnResourcesChanged((ResourceType)Enum.Parse(typeof(ResourceType), i.ToString()), loadGame.resources[i]);
        }

        tradeRatios = loadGame.tradeRatios;

        // Village Knight Roads 



    }

	[Command]
	public void CmdSendChatMsg(string msg){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		int p = l.IndexOf(this);
		RpcSendChatMsg (p, this.playerName, msg);
	}

	[ClientRpc]
	public void RpcSendChatMsg(int playerIndex, string name, string msg){
		List<Player> l = GameObject.Find("panelPlayers").GetComponent<PlayerList>()._players;
		if (GameObject.Find ("panelChat").GetComponent<ChatBoxFunction>().historyMsg.Count >= 5) {
			GameObject.Find ("panelChat").GetComponent<ChatBoxFunction>().historyMsg.Dequeue ();
		}
		GameObject.Find ("panelChat").GetComponent<ChatBoxFunction>().historyMsg.Enqueue (name + " : " + msg);
		string content = "";
		for (int i = 0; i < GameObject.Find ("panelChat").GetComponent<ChatBoxFunction>().historyMsg.Count; i++) {
			string buffer = GameObject.Find ("panelChat").GetComponent<ChatBoxFunction>().historyMsg.Dequeue ();
			content += buffer + "\n";
			GameObject.Find ("panelChat").GetComponent<ChatBoxFunction>().historyMsg.Enqueue (buffer);
		}
		GameObject.Find ("panelChat").GetComponent<ChatBoxFunction>().display = content;

		if (l [playerIndex].isLocalPlayer) {
			if (msg.Contains ("pgcard")) {	// "pgcard 00"
				int pgcType = 0;
				int x = 0;
				string number = ""+msg [(msg.IndexOf ('d') + 2)]+ msg[(msg.IndexOf ('d') + 3)];
				pgcType = Int32.Parse(number);
				GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();
				int cate = 0;
				if (pgcType >= 0 && pgcType <= 9) {
					cate = 2;
				} else if (pgcType >= 10 && pgcType <= 18) {
					cate = 1;
				}
				board.panelProgressCard.GetComponent<ProgressCardList> ().Add (cate, pgcType, true);
			}
			if (msg.Contains ("resource")) { // "resource x" (10 items per time)
				string number = ""+msg [(msg.IndexOf ('c') + 3)];
				int type = Int32.Parse (number);
				l [playerIndex].OnResourcesChanged ((ResourceType)type, 10);
			}
		}

	}

}
