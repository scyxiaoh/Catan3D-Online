using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


// TODO destory the card after use and then put it back
// TODO check vp card when add it
public class ProgressCardList : MonoBehaviour {

	private bool isSetUp = false;
	public RectTransform slots;
	public GameObject panelCardDisplay;
	public Image displayTitleBar;
	public Text displayTitle;
	public Text displayContent;
	public Button buttonConfirmation;
	public Button buttonCancellation;
	public GameObject ProgressCardObj;
	public Sprite[] titleBarSprites = new Sprite[3];
	public List<String> pgCardDescription = new List<String> ();

	public List<ProgressCard> cards = new List<ProgressCard>();

	public delegate void ProgressCardDelegate();
	public ProgressCardDelegate cardsDelegate;
	public void RunWaitingDelegate(){
		if (cardsDelegate != null) {
			cardsDelegate ();
			cardsDelegate = null;
		}
	}

	public void OnEnable(){
		if (!isSetUp) {
			progressDescription ();
			isSetUp = true;
		}
		foreach(ProgressCard p in cards){
			GameObject newCardObj = Instantiate (ProgressCardObj);
			newCardObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = titleBarSprites[(int)p.category];
			newCardObj.transform.GetChild (1).gameObject.GetComponent<Text> ().text = p.myType.ToString ();
			newCardObj.transform.SetParent (slots);
			Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
			newCardObj.GetComponent<Button> ().onClick.AddListener (delegate{
				DisplayCard(p.category, p.myType.ToString (), pgCardDescription[(int)p.myType], delegate{
					if(p.myType != pgCardType.Alchemist){
						playProgressCard(p);
					} else {
						GameObject.Find("GameBoard").GetComponent<GameBoard>().panelInfo.GetComponent<InfoPanel>().pushMessage(
							"You can't play Alchemist card after dices rolled.",null,null);
					}
				});														// add functionality later
			});

		}
	}

	public void OnDisable(){
		for (int i = slots.childCount-1; i >= 0; i--) {
			Destroy (slots.GetChild(i).gameObject);
		}
	}

	public void RequestDiscard(bool isMyturn){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		gameObject.SetActive (true);
		transform.GetChild (2).gameObject.GetComponent<Button> ().interactable = false;
		for (int i = slots.childCount-1; i >= 0; i--) {
			Destroy (slots.GetChild(i).gameObject);
		}
		foreach(ProgressCard p in cards){
			GameObject newCardObj = Instantiate (ProgressCardObj);
			newCardObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = titleBarSprites[(int)p.category];
			newCardObj.transform.GetChild (1).gameObject.GetComponent<Text> ().text = p.myType.ToString ();
			newCardObj.transform.SetParent (slots);
			newCardObj.GetComponent<Button> ().onClick.AddListener (delegate{
				DisplayCard(p.category, p.myType.ToString (), pgCardDescription[(int)p.myType], delegate{
					if (p.myType == 0) {
						localPlayer.CmdNotifyHasAlchemist(false);
					}
					cards.Remove(p);
					localPlayer.CmdDiscardProgressCard((int)p.category, (int)p.myType);
					if (!isMyturn){
						localPlayer.CmdResponseToServer();
					}
					transform.GetChild (2).gameObject.GetComponent<Button> ().interactable = true;
				});														// add functionality later
			});
		}
	}

	public void Add(int category, int cardType, bool isMyturn){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		ProgressCard newCard = new ProgressCard ();
		newCard = newCard.setPg ((pgCardType)Enum.Parse (typeof(pgCardType), cardType.ToString ()), category);

		if (cardType == (int)pgCardType.Constitution || cardType == (int)pgCardType.Printer) {
			playProgressCard (newCard);
		} else {
			cards.Add (newCard);
			if (cardType == 0) {
				localPlayer.CmdNotifyHasAlchemist(true);
			}
			if (localPlayer.progressCardSum < 4) {
				localPlayer.CmdProgressCardSumChange (1);
				if (!isMyturn) {
					localPlayer.CmdResponseToServer ();
				}
			} else {
				localPlayer.CmdProgressCardSumChange (1);
				RequestDiscard (isMyturn);
			}
		}
	}

	public void DisplayCard(Improvement category, string title, string content, ProgressCardDelegate functionality){
		displayTitleBar.sprite = titleBarSprites [(int)category];
		displayTitle.text = title;
		displayContent.text = content;
		cardsDelegate = functionality;
		panelCardDisplay.SetActive (true);
	}

	public void playCardOfType(int pgType){
		foreach (ProgressCard p in cards) {
			if ((int)p.myType == pgType) {
				playProgressCard (p);
			}
		}
	}

	// all the function is in this method
	public void playProgressCard(ProgressCard card){
		pgCardType name = card.myType;
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		GameBoard board = GameObject.Find ("GameBoard").GetComponent<GameBoard> ();

		if (name == pgCardType.Crane) {
			localPlayer.isCrane = true;
		} else if (name == pgCardType.Engineer) {
			localPlayer.isEnginner = true;
		} else if (name == pgCardType.Irrigation) {
			foreach (GameObject h in board.tiles) {
				if (h.GetComponent<Hex> ().Product () == ResourceType.Grain) {
					foreach (Vertex v in h.GetComponent<Hex>().adjacentVertices) {
						if (v != null && v.transform.childCount > 1 && v.transform.GetChild (1).GetComponent<Village> () != null && v.transform.GetChild (1).GetComponent<Village> ().owner == localPlayer) {
							localPlayer.OnResourcesChanged (ResourceType.Grain, 2);
							break;
						}
					}
				}
			}
		} else if (name == pgCardType.Medicine) {
			localPlayer.isMedicine = true;
		} else if (name == pgCardType.Mining) {
			foreach (GameObject h in board.tiles) {
				if (h.GetComponent<Hex> ().Product () == ResourceType.Ore) {
					foreach (Vertex v in h.GetComponent<Hex>().adjacentVertices) {
						if (v != null && v.transform.childCount > 1 && v.transform.GetChild (1).GetComponent<Village> () != null && v.transform.GetChild (1).GetComponent<Village> ().owner == localPlayer) {
							localPlayer.OnResourcesChanged (ResourceType.Ore, 2);
							break;
						}
					}
				}
			}
		} else if (name == pgCardType.Printer) {
			localPlayer.CmdVpChange (1);
		} else if (name == pgCardType.Road_Building) {
			localPlayer.roadBuilding = 2;
			board.RequestPlayRoadBuildingCard ();
		} else if (name == pgCardType.Smith) {
			localPlayer.isSmith += 2;
		} else if (name == pgCardType.Bishop) {
			board.RequestBishop ();
		} else if (name == pgCardType.Constitution) {
			localPlayer.CmdVpChange (1);
		} else if (name == pgCardType.Deserter) {
			// too complicated so as Diplomat & Intrigue
		} else if (name == pgCardType.Saboteur) {
			for (int i = 0; i < l.Count; i++) {
				if (l [i] != localPlayer & l [i].vP >= localPlayer.vP) {
					localPlayer.CmdRequestDiscardResources (i, Mathf.FloorToInt (l [i].resourceSum / 2));
				}
			}
		} else if (name == pgCardType.Warlord) {
			localPlayer.isWarLord = localPlayer.knights.Count;
			foreach (Knight k in localPlayer.knights) {
				if (!k.isActive) {
					localPlayer.CmdSetKnightActive (k.vertexIndex, true); // TODO resources
				}
			}
		} else if (name == pgCardType.Wedding) {
			localPlayer.CmdPlayedWeddingCard ();
		} else if (name == pgCardType.Master_Merchant) {
			foreach (Player p in l) {
				if (p != localPlayer) {
					if (p.vP > localPlayer.vP) {
						//to do: ui to make them choose player and which resources to take
						int num = 0; // THIS SHOULD BE REPLACED
						ResourceType chosen = 0;
						p.OnResourcesChanged (chosen, -num);
						localPlayer.OnResourcesChanged (chosen, num);
						localPlayer.masterMerchant = true;
					}
				}
			}
		} else if (name == pgCardType.Resource_Monopoly) {
			board.panelSelection.GetComponent<SelectionPanel> ().RequestResourceMonopoly ();
		} else if (name == pgCardType.Trade_Monopoly) {
			board.panelSelection.GetComponent<SelectionPanel> ().RequestTradeMonopoly ();
		} else if (name == pgCardType.Merchant_Fleet) {
			//to do: set the trade ratio to 2 for one turn 
			localPlayer.merchantFleet = true;
		} else if (name == pgCardType.Commercial_Harbor) {
			//to do: force other players to trade 1:1
			localPlayer.commercialHarbor = true;
		} else if (name == pgCardType.Spy) {
			bool hasCardsToTake = false;
			foreach (Player p in l) {
				if (p.progressCardSum > 0 && !p.isLocalPlayer) {
					hasCardsToTake = true;
				}
			}
			if (hasCardsToTake) {
				board.PlaySpyCard ();
			} else {
				board.panelInfo.GetComponent<InfoPanel> ().pushMessage ("Other players don't have any progress cards to let you take.", null, null);
				return;
			}

		} else if (name == pgCardType.Merchant) {
			board.RequestMoveMerchant ();
		} else if (name == pgCardType.Inventor) {
			board.RequestInventorChoice ();
		} else if (name == pgCardType.Alchemist) {
			board.panelSelection.GetComponent<SelectionPanel> ().RequestDiceSelection ();
			localPlayer.CmdNotifyHasAlchemist (false);
		}
		localPlayer.CmdAnnounce (localPlayer.playerName+" has played a " + card.myType.ToString() + " card!");
		cards.Remove(card);
		localPlayer.CmdDiscardProgressCard((int)card.category,(int)card.myType);

	}

	public void RevealProgressCardsTo(int toPlayerIndex){
		List<int> categories = new List<int> ();
		List<int> cardTypes = new List<int> ();
		foreach (ProgressCard p in cards) {
			categories.Add ((int)p.category);
			cardTypes.Add ((int)p.myType);
		}
		GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdRevealProgressCardToPlayer (toPlayerIndex, categories.ToArray (), cardTypes.ToArray ());
	}

	public void LookAndChooseCards(int fromPlayerIndex, int[] categories, int[] cardTypes){
		Debug.Log (categories.Length +" " +cardTypes.Length);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
		gameObject.SetActive (true);
		transform.GetChild (2).gameObject.GetComponent<Button> ().interactable = false;
		for (int i = slots.childCount-1; i >= 0; i--) {
			Destroy (slots.GetChild(i).gameObject);
		}
		List<ProgressCard> tempList = new List<ProgressCard> ();
		for (int i = 0; i < cardTypes.Length; i++) {
			ProgressCard newCard = new ProgressCard ();
			newCard = newCard.setPg ((pgCardType)cardTypes[i],categories[i]);
			tempList.Add (newCard);
		}
		foreach (ProgressCard p in tempList) {
			GameObject newCardObj = Instantiate (ProgressCardObj);
			newCardObj.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = titleBarSprites[(int)p.category];
			newCardObj.transform.GetChild (1).gameObject.GetComponent<Text> ().text = p.myType.ToString();
			newCardObj.transform.SetParent (slots);
			newCardObj.GetComponent<Button> ().onClick.AddListener (
				delegate{
					DisplayCard(p.category, p.myType.ToString (), pgCardDescription[(int)p.myType], 
						delegate{
							Add((int)p.category,(int)p.myType,true);
							localPlayer.CmdTakeProgressCard(fromPlayerIndex, (int)p.myType);
							localPlayer.CmdAnnounce(localPlayer.playerName + " has taken " +l[fromPlayerIndex].playerName + "'s " + 
								p.myType.ToString () + " card!" );
							transform.GetChild (2).gameObject.GetComponent<Button> ().interactable = true;
						});
				});	
		}

	}

	public void CardTaken(int cardType){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		foreach (ProgressCard p in cards) {
			if ((int)p.myType == cardType) {
				cards.Remove (p);
				localPlayer.CmdProgressCardSumChange (-1);
			}
		}
	}

	public void progressDescription(){
		pgCardDescription.Add ("Play this card before you roll all the dice. You decide what the result of both numbered dice will be. Then roll the event die normally. Resolve the event die first.");
		pgCardDescription.Add ("One city improvement that you build this turn costs one less commodity than usual.");
		pgCardDescription.Add ("You may build 1 city wall for free.");
		pgCardDescription.Add ("You may swap 2 number tokes of your choice on the board. You may not choose any 2, 12, 6 or 8 tokens.");
		pgCardDescription.Add ("You make take 2 grain cards from the bank for each fields hex which is adjacent to at least one of your cities or settlements.");
		pgCardDescription.Add ("For 2 ore and 1 grain, you may upgrade one of your settlements into a city");
		pgCardDescription.Add ("You may take 2 ore cards from the bank for each mountain hex adjacent to at least one of your cities or settlements.");
		pgCardDescription.Add ("Reveal this card immediately when you get it. This card cannot be stolen by a spy.");
		pgCardDescription.Add ("When you play this card you may place 2 roads for free (if playing Catan: Seafarers you may place 2 ships or 1 ship and 1 road instead).");
		pgCardDescription.Add ("You may promote up to 2 of your knights for free (the normal rules for promoting knights still apply). Mighty knights may not be promoted.");
		pgCardDescription.Add ("Move the robber. You may draw 1 random card (resource or commodity) from the hand of each player who has a settlement or city adjacent to the robber's new hex.");
		pgCardDescription.Add ("Reveal this card immediately when you draw it. This card cannot be stolen by a spy.");
		pgCardDescription.Add ("Choose an opponent. He must remove one of his knights (his choice) from the board. You may place 1 of your knights of on the board (its strength must be equal to the knight removed.");
		pgCardDescription.Add ("You may remove any open road (a road with nothing attached on one end). If you remove one of your roads you may place it in another location. ");
		pgCardDescription.Add ("You may displace one of your opponent’s knights, without using a knight of your own. The knight must be on an intersection connected to one of your roads or lines of ships.");
		pgCardDescription.Add ("Each player who has as many or more victory points than you must discard half his cards to the bank (resource and/or commodity).");
		pgCardDescription.Add ("Examine an opponent’s hand of progress cards. You may take 1 card of your choice and add it to your hand.");
		pgCardDescription.Add ("You may activate all of your knights for free.");
		pgCardDescription.Add ("Each player who has more victory points than you must give you 2 cards of his choice (resource and/or commodity).");
		pgCardDescription.Add ("You may offer each opponent a resource card from your hand. In exchange, each player must give you a commodity card of his choice. If he has none, your resource card is returned.");
		pgCardDescription.Add ("Select an opponent who has more victory points than you. You may examine his hand of resource cards and select any 2 cards, which you may add to your hand.");
		pgCardDescription.Add ("Place the merchant on a land hex next to your settlement or city. While the merchant remains here, you may trade the resource produced by this terrain at a 2:1 ratio.");
		pgCardDescription.Add ("For the rest of your turn, you may trade one resource or commodity of your choice with the bank at a 2:1 rate. You may make as many trades as you wish.");
		pgCardDescription.Add ("Name a resource (brick, grain, ore, lumber, or wool). Each opponent must give you 2 cards of that type (if he has any).");
		pgCardDescription.Add ("Name a commodity (cloth, coin, or paper). Each opponent must give you 1 card of that type (if he has any).");
	}
}
