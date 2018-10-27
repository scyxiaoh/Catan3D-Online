using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Game : NetworkBehaviour {
	
	private PlayerList myPlayers;
	public Player currentPlayer;
	private Player hostPlayer;
	public GamePhase currentPhase;

	public int numberOfPlayer;
	public int VPsToWin;
	private int barbarianPosition;
	private int knightStrength;
	private int barbarianStrength;
	private bool barbarianAttacked;

	public bool hasAchievedTrade;
	public bool hasAchievedScience;
	public bool hasAchievedPolitics;

	MySaveGame sv = new MySaveGame();


	public int yellowDie;
	public int redDie;
	public int eventDie; // 1-3 pirate 4 yellow(trade) 5 blue(politics) 6 green(science)


	public bool isWaiting =  false;
	public int selectionIndicator = -1;
	private bool started = false;
	private int defenderRemaining = 6;


	public Queue<ProgressCard> tradeCardsQueue = new Queue<ProgressCard>();
	public Queue<ProgressCard> politicsCardsQueue = new Queue<ProgressCard>();
	public Queue<ProgressCard> scienceCardsQueue = new Queue<ProgressCard>();



	public void Start(){
		myPlayers = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ();
		currentPhase = GamePhase.LoadingPlayers;
	}

	public void Update(){
		if (myPlayers._players.Count == numberOfPlayer&&currentPhase == GamePhase.LoadingPlayers) {
			//make sure all players are really set up
			bool ready = true;
			foreach (Player p in myPlayers._players) {
				if (p.isReady == false){
					ready = false;
				}
			}
			if (ready) {
				currentPlayer = myPlayers._players [0];
				hostPlayer = myPlayers._players [0];
				currentPhase = GamePhase.SetupRoundOne;
				OnBarbarianPositionChanged (0);
				OnBarbarianStrengthChanged (0);
				OnKnightStrengthChanged(0);
				ProgressCard helper = new ProgressCard ();
				tradeCardsQueue = helper.generateDeck (0);
				politicsCardsQueue = helper.generateDeck (1);
				scienceCardsQueue = helper.generateDeck (2);
			}
		}
		if (!started &&!isWaiting && currentPhase == GamePhase.SetupRoundOne) {
			StartCoroutine (TurnRotationCoroutine ());
			started = true;
		}
	}



	public void OnTurnEnd(){
		if (currentPhase == GamePhase.SetupRoundOne) {
			if (myPlayers._players.IndexOf (currentPlayer) >= myPlayers._players.Count - 1) {
				currentPhase = GamePhase.SetupRoundTwo;
			} else {
				currentPlayer = myPlayers._players [myPlayers._players.IndexOf (currentPlayer) + 1];
			}
		} else if (currentPhase == GamePhase.SetupRoundTwo) {
			if (myPlayers._players.IndexOf (currentPlayer) <= 0) {
				currentPhase = GamePhase.TurnFirstPhase;
			} else {
				currentPlayer = myPlayers._players [myPlayers._players.IndexOf (currentPlayer) - 1];
			}
		} else {
			if (myPlayers._players.IndexOf (currentPlayer) >= myPlayers._players.Count - 1) {
				currentPlayer = myPlayers._players [0];
			} else {
				currentPlayer = myPlayers._players [myPlayers._players.IndexOf (currentPlayer) + 1];
			}
			currentPhase = GamePhase.TurnFirstPhase;
		}
		isWaiting = false;
	}

	public void Initiate(int number, int vp){
		numberOfPlayer = number;
		VPsToWin = vp;
		barbarianAttacked = false;
		barbarianPosition = 7;
		barbarianStrength = 0;
		knightStrength = 0;
	//	longestRoadPlayer = -1;

		hasAchievedTrade = false;
		hasAchievedScience = false;
		hasAchievedPolitics = false;

	}

	public void OnBarbarianPositionChanged(int i){
		barbarianPosition += i;
		hostPlayer.RpcBarbarianPositionUpdate (barbarianPosition);
	}

	public void OnBarbarianStrengthChanged(int i){
		barbarianStrength += i;
		hostPlayer.RpcBarbarianStrengthUpdate (barbarianStrength, (barbarianStrength > knightStrength));
	}

	public void OnKnightStrengthChanged(int i){
		knightStrength += i;
		hostPlayer.RpcKnightStrengthUpdate (knightStrength, (barbarianStrength > knightStrength));
	}

	public IEnumerator TurnRotationCoroutine(){
		while (currentPhase == GamePhase.SetupRoundOne) {
			hostPlayer.TargetRequestBuildSettlementAndRoad (currentPlayer.connectionToClient);
			isWaiting = true;
			yield return new WaitWhile(()=>isWaiting);
		}
		while (currentPhase == GamePhase.SetupRoundTwo) {
			hostPlayer.TargetRequestBuildCityAndRoad (currentPlayer.connectionToClient);
			isWaiting = true;
			yield return new WaitWhile(()=>isWaiting);
		}
		while (currentPhase != GamePhase.Completed) {

			yellowDie = UnityEngine.Random.Range (1, 7); 
			redDie = UnityEngine.Random.Range (1, 7);
			eventDie = UnityEngine.Random.Range (1, 7);

			if (currentPlayer.hasAlchemist) {
				currentPlayer.TargetAskPlayAlchemistCard (currentPlayer.connectionToClient);
				isWaiting = true;
				yield return new WaitWhile (() => isWaiting);
			} 
			// Display the dice
			hostPlayer.RpcRollDice (yellowDie, redDie, eventDie);
			// If so, ask the player for the number of yellowDie and redDie, roll eventDie normally

			/////////////////////////////
			// check barbarian
			if (eventDie == 1 || eventDie == 2 || eventDie == 3) {
				OnBarbarianPositionChanged (-1);

				// barbarian attack
				if (barbarianPosition == 0) {

					// catan wins!
					if (barbarianStrength <= knightStrength) {
						hostPlayer.RpcAnnounce ("Barbarians arrive but they are defeated! ");
						int highestContribution = -1;
						int numofDefender = 0;
						// get the highest contribution and how many people deserve the reward
						foreach (Player p in myPlayers._players) {
							int contribution = 0;
							foreach (Knight k in p.knights) {
								if (k.isActive) {
									contribution += k.level;
								}
							}
							if (contribution > highestContribution) {
								highestContribution = contribution;
								numofDefender = 1;
							} else if (contribution == highestContribution) {
								numofDefender++;
							}
						}
						// count each contribution of the players and give the reward
						int currentPlayerIndex = myPlayers._players.IndexOf (currentPlayer);
						Player playerRotated;
						for (int i = currentPlayerIndex; i < myPlayers._players.Count + currentPlayerIndex; i++) {
							if (i < myPlayers._players.Count) {
								playerRotated = myPlayers._players [i];
							} else {
								playerRotated = myPlayers._players [i - myPlayers._players.Count];
							}

							int contribution = 0;
							foreach (Knight k in playerRotated.knights) {
								if (k.isActive) {	
									contribution += k.level;
									GameObject itsVertex = k.gameObject.transform.parent.gameObject;
									int vertexIndex = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions.IndexOf (itsVertex);
									playerRotated.CmdSetKnightActive (vertexIndex, false);
								}
							}
							// get the reward
							if (contribution == highestContribution) {
								// Defender of catan
								if (numofDefender == 1) {
									if (defenderRemaining >= 0) {
										hostPlayer.RpcAnnounce (playerRotated.playerName + " is declared to be the “Defender of Catan” and receive 1 VP!");
										playerRotated.CmdDefendCountChange (1);
										defenderRemaining--;
									} else {
										hostPlayer.RpcAnnounce ("No more Defender of Catan available");
									}
								} else {
									hostPlayer.RpcAnnounce (playerRotated.playerName + " can draw a progress card.");
									playerRotated.TargetRequestDrawProgressCard (playerRotated.connectionToClient, (tradeCardsQueue.Count > 0), (politicsCardsQueue.Count > 0), (scienceCardsQueue.Count > 0));
									selectionIndicator = 3;
									yield return new WaitUntil(()=>(selectionIndicator>=0 && selectionIndicator <=2));
									ProgressCard drawn = new ProgressCard ();
									if (selectionIndicator == 0) {
										drawn = tradeCardsQueue.Dequeue ();
									} else if (selectionIndicator == 1) {
										drawn = politicsCardsQueue.Dequeue ();
									} else if (selectionIndicator == 2) {
										drawn = scienceCardsQueue.Dequeue ();
									}
									playerRotated.TargetGetProgressCard (playerRotated.connectionToClient, (int)drawn.category, (int)drawn.myType);
									isWaiting = true;
									yield return new WaitWhile (() => isWaiting);
								}
							}

						}
						/*
						foreach (Player p in myPlayers._players) {
							int contribution = 0;
							foreach (Knight k in p.knights) {
								if (k.isActive) {	
									contribution += k.level;
									GameObject itsVertex = k.gameObject.transform.parent.gameObject;
									int vertexIndex = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions.IndexOf (itsVertex);
									p.CmdSetKnightActive (vertexIndex, false);
								}
							}
							// get the reward
							if (contribution == highestContribution) {
								// Defender of catan
								if (numofDefender == 1) {
									if (defenderRemaining >= 0) {
										hostPlayer.RpcAnnounce ("Barbarians arrive but they are defeated! ");
										hostPlayer.RpcAnnounce (p.playerName + " is declared to be the “Defender of Catan” and receive 1 VP!");
										p.CmdDefendCountChange (1);
										defenderRemaining--;
									}
								} else {
									// TODO PgCard
									// TODO grabProgressCard(type) UI is needed
									// if # of pgcard > 4, give up one card and pu ot back to deck
								}
							}
						}
						*/

					} else {    // catan lose
						hostPlayer.RpcAnnounce ("Barbarians arrive and they defeat the force of Catan!");
						int lowestContribution = 0;
						foreach (Player p in myPlayers._players) {
							int contribution = 0;
							foreach (Knight k in p.knights) {
								if (k.isActive) {
									contribution += k.level;
								}
							}
							int numOfCities = 0;
							foreach (Village v in p.villages) {
								if (v.vt == VillageType.City) {
									numOfCities++;
								}
							}
							if (numOfCities > 0 && (contribution < lowestContribution)) {
								lowestContribution = contribution;
							}
						}
						foreach (Player p in myPlayers._players) {
							int contribution = 0;
							foreach (Knight k in p.knights) {
								if (k.isActive) {
									contribution += k.level;
									GameObject itsVertex = k.gameObject.transform.parent.gameObject;
									int vertexIndex = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().junctions.IndexOf (itsVertex);
									p.CmdSetKnightActive (vertexIndex, false);
								}
							}
							if (contribution == lowestContribution) {
								int numOfCities = 0;
								foreach (Village v in p.villages) {
									if (v.vt == VillageType.City) {
										numOfCities++;
									}
								}
								if (numOfCities > 0) {
									hostPlayer.RpcAnnounce (p.playerName + " is raided!");
									p.TargetRequestDowngradeCity (p.connectionToClient);
									isWaiting = true;
									yield return new WaitWhile (() => isWaiting);
								} else {
									hostPlayer.RpcAnnounce (p.playerName + " is raided, but doesn't have any city to be downgraded.");
								}
							}
						}
					}
					OnBarbarianPositionChanged (7);
					barbarianAttacked = true;
				}
			} else if (eventDie == 4) {
				int currentPlayerIndex = myPlayers._players.IndexOf (currentPlayer);
				Player p;
				for (int i = currentPlayerIndex; i < myPlayers._players.Count + currentPlayerIndex; i++) {
					if (i < myPlayers._players.Count) {
						p = myPlayers._players [i];
					} else {
						p = myPlayers._players [i - myPlayers._players.Count];
					}
					if (p.fcTradeLvl >= redDie && p.fcTradeLvl > 1) {
						if (tradeCardsQueue.Count > 0) {
							ProgressCard drawn = tradeCardsQueue.Dequeue ();
							p.TargetGetProgressCard (p.connectionToClient, (int)drawn.category, (int)drawn.myType);
							hostPlayer.CmdAnnounce (p.playerName + " gets a Trade progress card");
							isWaiting = true;
							yield return new WaitWhile (() => isWaiting);
						} else {
							hostPlayer.CmdAnnounce (p.playerName + " could have got a Trade progress card, but the deck is empty.");
						}
					}
				}
			} else if (eventDie == 5) {
				int currentPlayerIndex = myPlayers._players.IndexOf (currentPlayer);
				Player p;
				for (int i = currentPlayerIndex; i < myPlayers._players.Count + currentPlayerIndex; i++) {
					if (i < myPlayers._players.Count) {
						p = myPlayers._players [i];
					} else {
						p = myPlayers._players [i - myPlayers._players.Count];
					}
					if (p.fcPoliticsLvl >= redDie && p.fcPoliticsLvl > 1) {
						if (politicsCardsQueue.Count > 0) {
							ProgressCard drawn = politicsCardsQueue.Dequeue ();
							p.TargetGetProgressCard (p.connectionToClient, (int)drawn.category, (int)drawn.myType);
							hostPlayer.CmdAnnounce (p.playerName + " gets a Politics progress card");
							isWaiting = true;
							yield return new WaitWhile (() => isWaiting);
						} else {
							hostPlayer.CmdAnnounce (p.playerName + " could have got a Politics progress card, but the deck is empty.");
						}
					}
				}
			} else if (eventDie == 6) {
				int currentPlayerIndex = myPlayers._players.IndexOf (currentPlayer);
				Player p;
				for (int i = currentPlayerIndex; i < myPlayers._players.Count + currentPlayerIndex; i++) {
					if (i < myPlayers._players.Count) {
						p = myPlayers._players [i];
					} else {
						p = myPlayers._players [i - myPlayers._players.Count];
					}
					if (p.fcScienceLvl >= redDie && p.fcScienceLvl > 1) {
						if (scienceCardsQueue.Count > 0) {
							ProgressCard drawn = scienceCardsQueue.Dequeue ();
							p.TargetGetProgressCard (p.connectionToClient, (int)drawn.category, (int)drawn.myType);
							hostPlayer.CmdAnnounce (p.playerName + " gets a Science progress card");
							isWaiting = true;
							yield return new WaitWhile (() => isWaiting);
						} else {
							hostPlayer.CmdAnnounce (p.playerName + " could have got a Science progress card, but the deck is empty.");
						}
					}
				}
			}

			// Time for yellow die and red die!!
			int sum = yellowDie + redDie;
			if (sum == 7) {
				if (barbarianAttacked) {
					foreach (Player p in myPlayers._players) {
						if (p.resourceSum > (p.numOfCityWall * 2 + 7)) {
							hostPlayer.RpcAnnounce((p.playerName + " is robbed."));
							int numToDiscard = Mathf.FloorToInt((float)p.resourceSum/2f);
							p.TargetRequestDiscardResources (p.connectionToClient, numToDiscard);
							isWaiting = true;
							yield return new WaitWhile(()=>isWaiting);
						}
					}
					currentPlayer.TargetRequestMoveRobber (currentPlayer.connectionToClient);
					isWaiting = true;
					yield return new WaitWhile(()=>isWaiting);
				}
			} else {

				GameObject robberAt = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().robber.transform.parent.gameObject;

				foreach (Player p in myPlayers._players) {
					bool gotResource = false;
					foreach (Village v in p.villages) {
						foreach (Hex h in v.gameObject.transform.parent.GetComponent<Vertex>().adjacentHexes) {
							if (h!=null && h.hexNumber == sum && h.gameObject != robberAt) {
								ResourceType resourceType = h.Product();
								int type = (int)resourceType;
								if (v.vt == VillageType.Settlement) {
									if (type >= 0 && type <= 5) {
										p.TargetResourceChange (p.connectionToClient, type, 1);
										gotResource = true;
									} else if (type == 8) {
										p.TargetRequestChooseResource (p.connectionToClient);
										isWaiting = true;
										yield return new WaitWhile(()=>isWaiting);
										gotResource = true;
									}
								} else {
									if (type == 1 || type == 3) {
										p.TargetResourceChange (p.connectionToClient, type, 2);
										gotResource = true;
									} else if (type == 4) { // ore
										p.TargetResourceChange (p.connectionToClient, type, 1);
										p.TargetResourceChange (p.connectionToClient, 7, 1);
										gotResource = true;
									} else if (type == 2) { // wool
										p.TargetResourceChange (p.connectionToClient, type, 1);
										p.TargetResourceChange (p.connectionToClient, 6, 1);
										gotResource = true;
									} else if (type == 0) { // lumber
										p.TargetResourceChange (p.connectionToClient, type, 1);
										p.TargetResourceChange (p.connectionToClient, 5, 1);
										gotResource = true;
									} else if (type == 8) {
										p.TargetRequestChooseResource (p.connectionToClient);
										isWaiting = true;
										yield return new WaitWhile(()=>isWaiting);
										p.TargetRequestChooseResource (p.connectionToClient);
										isWaiting = true;
										yield return new WaitWhile(()=>isWaiting);
										gotResource = true;
									}
								}
							}
						}
					}
					if (!gotResource && p.fcScienceLvl >= 4) {
						p.TargetRequestChooseResource (p.connectionToClient);
						isWaiting = true;
						yield return new WaitWhile(()=>isWaiting);
					}
				}


			}

			currentPhase = GamePhase.TurnDiceRolled;
			hostPlayer.TargetGiveTurn (currentPlayer.connectionToClient);
			isWaiting = true;
			yield return new WaitWhile(()=>isWaiting);
		}
	}
		
	public void EndOfGame(int victoriousPlayerIndex){
		Player p = myPlayers._players [victoriousPlayerIndex];
		hostPlayer.RpcAnnounce (p.playerName + " wins the game!" );
		currentPhase = GamePhase.Completed;
	}

	// save game
	public void save(){
        string svname = GameObject.Find("panelMenu").gameObject.transform.GetChild(0).GetComponent<InputField>().text;
		GameBoard board = GameObject.Find ("GameBoard").gameObject.GetComponent<GameBoard> ();
		sv.numberOfPlayer = numberOfPlayer;
		Debug.Log (numberOfPlayer);
		Debug.Log (sv.numberOfPlayer);
		sv.VPsToWin = VPsToWin;
		sv.barbarianPosition = barbarianPosition;
		sv.barbarianAttacked = barbarianAttacked;
		sv.barbarianStrength = barbarianStrength;
		sv.knightStrength = knightStrength;
		sv.currentPhase = currentPhase;
        foreach(ProgressCard c in tradeCardsQueue)
        {
            sv.tradeCardsQueue.Add((int)c.myType);
        }
        foreach (ProgressCard c in politicsCardsQueue)
        {
            sv.politicsCardsQueue.Add((int)c.myType);
        }
        foreach (ProgressCard c in scienceCardsQueue)
        {
            sv.scienceCardsQueue.Add((int)c.myType);
        }

		foreach (int n in board.numbers) {
			sv.numbers.Add (n);
		}
		foreach (int n in board.terrains) {
			sv.terrains.Add (n);
		}
		foreach (int n in board.harbortypes) {
			sv.harbortypes.Add (n);
		}
		bool a = SaveGameSystem.SaveGame (sv, svname);
		Debug.Log (a);

		for (int i = 0; i < numberOfPlayer; i++) {
			myPlayers._players [i].SaveData (myPlayers._players[i].connectionToClient, svname);
		}


	}

	public void load(){
        string svname = GameObject.Find("panelMenu").gameObject.transform.GetChild(1).GetComponent<InputField>().text;
        MySaveGame loadGame = SaveGameSystem.LoadGame (svname) as MySaveGame;


        numberOfPlayer = loadGame.numberOfPlayer;
        VPsToWin = loadGame.VPsToWin;
        barbarianPosition = loadGame.barbarianPosition;
        barbarianAttacked = loadGame.barbarianAttacked;
        barbarianStrength = loadGame.barbarianStrength;
        knightStrength = loadGame.knightStrength;
        currentPhase = loadGame.currentPhase;
// pgcard


        List<int> number = loadGame.numbers;
        List<int> terrians = loadGame.terrains;
        List<int> harbortypes = loadGame.harbortypes;


        for (int i = 0; i < numberOfPlayer; i++)
        {
            myPlayers._players[i].LoadData(myPlayers._players[i].connectionToClient, svname);
        }
        Debug.Log (loadGame);
		Debug.Log (loadGame.numbers);


		// TODO load

	}



}









