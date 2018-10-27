using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : MonoBehaviour {

	private bool isSetUp = false;
	private bool isTradingWithPlayers = true;
	private bool isRespondingTradeOffers = false;
	public List<int> resourcesOffered = new List<int>();
	private List<int> tradeRatios = new List<int> ();
	private List<Text> resourcesDisplay = new List<Text>();
	private List<Text> resourcesOfferedDisplay = new List<Text>();
	private List<Text> resourcesDesiredDisplay = new List<Text>();
	private List<Button> buttonOffering = new List<Button>();
	private List<Button> buttonDesiring = new List<Button>();
	public int tradeInstancePrimary;
	public int tradeInstanceSecondary;
	public delegate void TradeDelegate();
	public TradeDelegate tradeDelegate;

	public void RunTradeDelegate(){
		if (tradeDelegate != null) {
			tradeDelegate ();
			tradeDelegate = null;
		}
	}

	private void OnEnable(){
		if (!isSetUp) {
			for (int i = 0; i < 9; i++) {
				if (i < 8) {
					resourcesDisplay.Add (transform.GetChild (0).GetChild (i).gameObject.GetComponent<Text> ());
					tradeRatios.Add (4);
				}
				resourcesOffered.Add (0);
				resourcesDesiredDisplay.Add (transform.GetChild (1).GetChild (i).gameObject.GetComponent<Text> ());
				resourcesOfferedDisplay.Add (transform.GetChild (2).GetChild (i).gameObject.GetComponent<Text> ());
				buttonDesiring.Add (transform.GetChild (3).GetChild (i).gameObject.GetComponent<Button> ());
				buttonOffering.Add (transform.GetChild (4).GetChild (i).gameObject.GetComponent<Button> ());
				switch (i) {
				case 0:
					buttonDesiring [i].onClick.AddListener (delegate {
						ModifyOffer (0, -1);
					});
					buttonOffering [i].onClick.AddListener (delegate {
						ModifyOffer (0, 1);
					});
					break;
				case 1:
					buttonDesiring [i].onClick.AddListener (delegate {
						ModifyOffer (1, -1);
					});
					buttonOffering [i].onClick.AddListener (delegate {
						ModifyOffer (1, 1);
					});
					break;
				case 2:
					buttonDesiring [i].onClick.AddListener (delegate {
						ModifyOffer (2, -1);
					});
					buttonOffering [i].onClick.AddListener (delegate {
						ModifyOffer (2, 1);
					});
					break;
				case 3:
					buttonDesiring [i].onClick.AddListener (delegate {
						ModifyOffer (3, -1);
					});
					buttonOffering [i].onClick.AddListener (delegate {
						ModifyOffer (3, 1);
					});
					break;
				case 4:
					buttonDesiring [i].onClick.AddListener (delegate {
						ModifyOffer (4, -1);
					});
					buttonOffering [i].onClick.AddListener (delegate {
						ModifyOffer (4, 1);
					});
					break;
				case 5:
					buttonDesiring [i].onClick.AddListener (delegate {
						ModifyOffer (5, -1);
					});
					buttonOffering [i].onClick.AddListener (delegate {
						ModifyOffer (5, 1);
					});
					break;
				case 6:
					buttonDesiring [i].onClick.AddListener (delegate {
						ModifyOffer (6, -1);
					});
					buttonOffering [i].onClick.AddListener (delegate {
						ModifyOffer (6, 1);
					});
					break;
				case 7:
					buttonDesiring [i].onClick.AddListener (delegate {
						ModifyOffer (7, -1);
					});
					buttonOffering [i].onClick.AddListener (delegate {
						ModifyOffer (7, 1);
					});
					break;
				case 8:
					buttonDesiring [i].onClick.AddListener (delegate {
						ModifyOffer (8, -1);
					});
					buttonOffering [i].onClick.AddListener (delegate {
						ModifyOffer (8, 1);
					});
					break;
				}
			}
			List<Player> l = GameObject.Find ("panelPlayers").GetComponent<PlayerList> ()._players;
			for (int i = 0; i < l.Count; i++) {
				transform.GetChild (7).GetChild (1 + i).GetChild (3).GetComponent<Text> ().text = l [i].playerName;
			}
			isSetUp = true;
		}
	}

	private void ModifyOffer(int resourceIndex, int change){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		if (isTradingWithPlayers) {
			if (resourceIndex < 8 && localPlayer.resources [resourceIndex] >= (resourcesOffered [resourceIndex] + change)) {
				resourcesOffered [resourceIndex] += change;
				if (resourcesOffered [resourceIndex] >= 0) {
					resourcesOfferedDisplay [resourceIndex].text = resourcesOffered [resourceIndex].ToString ();
					resourcesDesiredDisplay [resourceIndex].text = "0";
				} else {
					resourcesDesiredDisplay [resourceIndex].text = (0 - resourcesOffered [resourceIndex]).ToString ();
					resourcesOfferedDisplay [resourceIndex].text = "0";
				}
				resourcesDisplay [resourceIndex].text = (localPlayer.resources [resourceIndex] - resourcesOffered [resourceIndex]).ToString ();
			} else if (resourceIndex == 8) {
				resourcesOffered [resourceIndex] += change;
				if (resourcesOffered [resourceIndex] >= 0) {
					resourcesOfferedDisplay [resourceIndex].text = resourcesOffered [resourceIndex].ToString ();
					resourcesDesiredDisplay [resourceIndex].text = "0";
				} else {
					resourcesDesiredDisplay [resourceIndex].text = (0 - resourcesOffered [resourceIndex]).ToString ();
					resourcesOfferedDisplay [resourceIndex].text = "0";
				}
			}
			tradeInstancePrimary = 0; 
			tradeInstanceSecondary = 0;
			foreach (int numOffered in resourcesOffered) {
				if (numOffered >= 0) {
					tradeInstancePrimary += numOffered;
				} else {
					tradeInstanceSecondary -= numOffered;
				}
			}
			transform.GetChild (6).gameObject.GetComponent<Button> ().interactable = !(tradeInstancePrimary == 0 || tradeInstanceSecondary == 0);
			for (int i = 0; i < 8; i++) {
				if ((localPlayer.resources [i] - resourcesOffered [i]) < 0) {
					transform.GetChild (6).gameObject.GetComponent<Button> ().interactable = false;
				}
			}
			if (isRespondingTradeOffers && resourcesOffered [8] != 0) {
				transform.GetChild (6).gameObject.GetComponent<Button> ().interactable = false;
			}

		} else {
			int ratio = tradeRatios [resourceIndex];
			if (resourcesOffered [resourceIndex] > 0) {
				if (localPlayer.resources [resourceIndex] >= (resourcesOffered [resourceIndex] + change * ratio)) {
					resourcesOffered [resourceIndex] += change * ratio;
					resourcesOfferedDisplay [resourceIndex].text = resourcesOffered [resourceIndex].ToString ();
					resourcesDisplay [resourceIndex].text = (localPlayer.resources [resourceIndex] - resourcesOffered [resourceIndex]).ToString ();
					tradeInstancePrimary += change;
				}		
			} else if (resourcesOffered [resourceIndex] == 0) {
				if (change >= 0) {
					if (localPlayer.resources [resourceIndex] >= (resourcesOffered [resourceIndex] + change * ratio)) {
						resourcesOffered [resourceIndex] += change * ratio;
						resourcesOfferedDisplay [resourceIndex].text = resourcesOffered [resourceIndex].ToString ();
						tradeInstancePrimary += change;
					}	
				} else {
					resourcesOffered [resourceIndex] += change;
					resourcesDesiredDisplay [resourceIndex].text = (0 - resourcesOffered [resourceIndex]).ToString ();
					tradeInstancePrimary += change;
				}
				resourcesDisplay [resourceIndex].text = (localPlayer.resources [resourceIndex] - resourcesOffered [resourceIndex]).ToString ();
			} else {
				resourcesOffered [resourceIndex] += change;
				resourcesDesiredDisplay [resourceIndex].text = (0 - resourcesOffered [resourceIndex]).ToString ();
				resourcesDisplay [resourceIndex].text = (localPlayer.resources [resourceIndex] - resourcesOffered [resourceIndex]).ToString ();
				tradeInstancePrimary += change;
			}
			transform.GetChild (6).gameObject.GetComponent<Button> ().interactable = (tradeInstancePrimary==0);
		}
	}

	private void ForceModifyOffer(int resourceIndex, int change) {
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		if (resourceIndex < 8) {
			resourcesOffered [resourceIndex] += change;
			if (resourcesOffered [resourceIndex] >= 0) {
				resourcesOfferedDisplay [resourceIndex].text = resourcesOffered [resourceIndex].ToString ();
				resourcesDesiredDisplay [resourceIndex].text = "0";
			} else {
				resourcesDesiredDisplay [resourceIndex].text = (0 - resourcesOffered [resourceIndex]).ToString ();
				resourcesOfferedDisplay [resourceIndex].text = "0";
			}
			resourcesDisplay [resourceIndex].text = (localPlayer.resources [resourceIndex] - resourcesOffered [resourceIndex]).ToString ();
		} else if (resourceIndex == 8) {
			resourcesOffered [resourceIndex] += change;
			if (resourcesOffered [resourceIndex] >= 0) {
				resourcesOfferedDisplay [resourceIndex].text = resourcesOffered [resourceIndex].ToString ();
				resourcesDesiredDisplay [resourceIndex].text = "0";
			} else {
				resourcesDesiredDisplay [resourceIndex].text = (0 - resourcesOffered [resourceIndex]).ToString ();
				resourcesOfferedDisplay [resourceIndex].text = "0";
			}
		}
		tradeInstancePrimary = 0; 
		tradeInstanceSecondary = 0;
		foreach (int numOffered in resourcesOffered) {
			if (numOffered >= 0) {
				tradeInstancePrimary += numOffered;
			} else {
				tradeInstanceSecondary -= numOffered;
			}
		}
		transform.GetChild (6).gameObject.GetComponent<Button> ().interactable = !(tradeInstancePrimary == 0 || tradeInstanceSecondary == 0);
		for (int i = 0; i < 8; i++) {
			if ((localPlayer.resources [i] - resourcesOffered [i]) < 0) {
				transform.GetChild (6).gameObject.GetComponent<Button> ().interactable = false;
			}
		}
		if (isRespondingTradeOffers && resourcesOffered [8] != 0) {
			transform.GetChild (6).gameObject.GetComponent<Button> ().interactable = false;
		}
	}

	public void InitiateTradeOffer(){
		transform.GetChild (6).gameObject.GetComponent<Button> ().interactable = false;
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		for (int i = 0; i < 9; i++) {
			if (i < 8) {
				resourcesDisplay[i].text = localPlayer.resources[i].ToString();
			}
			resourcesOffered [i] = 0;
			resourcesOfferedDisplay [i].text = "0";
			resourcesDesiredDisplay [i].text = "0";
		}
	}

	public void PrepareTradeWithPlayer(){
		foreach (Button b in buttonOffering) {
			b.interactable = true;
		}
		foreach (Button b in buttonDesiring) {
			b.interactable = true;
		}
		tradeDelegate = OfferTradeToPlayers;
		isTradingWithPlayers = true;
		tradeInstancePrimary = 0;
		tradeInstanceSecondary = 0;
		Transform tradingWithPlayers = transform.GetChild (7);
		tradingWithPlayers.GetChild (0).gameObject.GetComponent<Button> ().interactable = true;
		for (int i = 1; i < 5; i++) {
			tradingWithPlayers.GetChild (i).gameObject.SetActive (false);
			for (int j = 0; j < tradingWithPlayers.GetChild (i).childCount-1; j++) {
				tradingWithPlayers.GetChild (i).GetChild (j).gameObject.SetActive (false);
				tradingWithPlayers.GetChild (i).GetChild (j).gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
			}
		}
	}

	public void PrepareTradeWithBack(){
		tradeDelegate = OfferTradeToBank;
		isTradingWithPlayers = false;
		tradeInstancePrimary = 0;
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		for (int i = 0; i < 8; i++) {
			transform.GetChild (8).GetChild (i + 1).gameObject.GetComponent<Text> ().text = "1:" + localPlayer.tradeRatios [i].ToString();
			tradeRatios [i] = localPlayer.tradeRatios [i];
		}
		if (localPlayer.hasMerchant) {
			Hex merchantAt = GameObject.Find ("GameBoard").GetComponent<GameBoard> ().merchant.transform.parent.gameObject.GetComponent<Hex> ();
			transform.GetChild (8).GetChild ((int)merchantAt.Product () + 1).gameObject.GetComponent<Text> ().text = "1:2";
			tradeRatios [(int)merchantAt.Product ()] = 2;
		}
		if (localPlayer.merchantFleet) {
			for (int i = 0; i < 8; i++) {
				transform.GetChild (8).GetChild (i + 1).gameObject.GetComponent<Text> ().text = "1:2";
				tradeRatios [i] = 2;
			}
		}
		buttonDesiring [8].interactable = false;
		buttonOffering [8].interactable = false;
	}

	private void OfferTradeToBank(){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.processTrade (resourcesOffered);
		gameObject.SetActive (false);
	}

	private void OfferTradeToPlayers(){
		transform.GetChild (7).GetChild (0).gameObject.GetComponent<Button> ().interactable = false;
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		localPlayer.CmdGiveTradeOffer (resourcesOffered.ToArray ());
		foreach (Button b in buttonOffering) {
			b.interactable = false;
		}
		foreach (Button b in buttonDesiring) {
			b.interactable = false;
		}
		transform.GetChild (6).gameObject.GetComponent<Button> ().interactable = false;
	}
		
	public void ReceiveTradeOffer(int[] resourcesDesired, int fromIndex){
		isRespondingTradeOffers = true;
		gameObject.SetActive (true);
		transform.GetChild (7).gameObject.SetActive (true);
		transform.GetChild (8).gameObject.SetActive (false);
		InitiateTradeOffer ();
		PrepareTradeWithPlayer ();
		transform.GetChild (7).GetChild (0).gameObject.GetComponent<Button> ().interactable = false;
		for (int i = 0; i < 9; i++) {
			ForceModifyOffer (i, -resourcesDesired [i]);
		}
		//show where does the offer come from
		transform.GetChild (7).GetChild (1 + fromIndex).gameObject.SetActive (true);
		transform.GetChild (7).GetChild (1 + fromIndex).GetChild(2).gameObject.SetActive (true);

		tradeDelegate = delegate {
			GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdResponseTradeOffer(resourcesOffered.ToArray(), fromIndex);
			gameObject.SetActive (false);
			isRespondingTradeOffers = false;
		};
		List<int> tempList = new List<int> ();
		for (int i = 0; i < 8; i++) {
			tempList.Add (0);
		}
		transform.GetChild (5).gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
			GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdResponseTradeOffer(tempList.ToArray(), fromIndex);
			transform.GetChild (5).gameObject.GetComponent<Button> ().onClick.RemoveAllListeners();
			isRespondingTradeOffers = false;
		});
	}

	public void ShowOfferResponse(int[] resourcesDesired, int fromIndex){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		transform.GetChild (6).gameObject.GetComponent<Button> ().interactable = true;
		for (int i = 0; i < 8; i++) {
			if (localPlayer.resources [i] + resourcesDesired [i] < 0) {
				transform.GetChild (6).gameObject.GetComponent<Button> ().interactable = false;
			}
			resourcesDisplay [i].text = (localPlayer.resources [i] + resourcesDesired [i]).ToString ();
			if (resourcesDesired [i] >= 0) {
				resourcesDesiredDisplay [i].text = resourcesDesired [i].ToString ();
				resourcesOfferedDisplay [i].text = "0";
			} else {
				resourcesOfferedDisplay [i].text = (-resourcesDesired [i]).ToString ();
				resourcesDesiredDisplay [i].text = "0";
			}
		}
		resourcesDesiredDisplay [8].text = "0";
		resourcesOfferedDisplay [8].text = "0";
		tradeDelegate = delegate {
			localPlayer.CmdAcceptTradeOffer(resourcesDesired, fromIndex);
			gameObject.SetActive (false);
		};
	}

	public void ReceiveOfferResponse(int[] resourcesDesired, int fromIndex) {
		transform.GetChild (7).GetChild (1 + fromIndex).gameObject.SetActive (true);
		bool isAcceptance = true;
		bool isRejection = true;
		for (int i = 0; i < 8; i++) {
			if (resourcesOffered [i] != -resourcesDesired [i]) {
				isAcceptance = false;
			}
			if (resourcesDesired [i] != 0) {
				isRejection = false;
			}
		}
		if (isRejection) {
			transform.GetChild (7).GetChild (1 + fromIndex).GetChild(1).gameObject.SetActive (true);
			transform.GetChild (7).GetChild (1 + fromIndex).GetChild (1).gameObject.GetComponent<Button> ().onClick.RemoveAllListeners();
		} else if (isAcceptance) {
			transform.GetChild (7).GetChild (1 + fromIndex).GetChild(0).gameObject.SetActive (true);
			transform.GetChild (7).GetChild (1 + fromIndex).GetChild (0).gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
				ShowOfferResponse(resourcesDesired, fromIndex);
			});
		} else {
			transform.GetChild (7).GetChild (1 + fromIndex).GetChild(2).gameObject.SetActive (true);
			transform.GetChild (7).GetChild (1 + fromIndex).GetChild (2).gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
				ShowOfferResponse(resourcesDesired, fromIndex);
			});
		}
	}
}
