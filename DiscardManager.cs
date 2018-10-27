using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscardManager : MonoBehaviour {

	private bool isSetUp = false;
	public List<int> resourcesOffered = new List<int>();
	private List<Text> resourcesDisplay = new List<Text>();
	private List<Text> resourcesOfferedDisplay = new List<Text>();
	private List<Button> buttonOffering = new List<Button>();
	private List<Button> buttonDesiring = new List<Button>();
	public int resourceSumRequired;
	public int discardingSum;

	private void OnEnable(){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		if (!isSetUp) {
			for (int i = 0; i < 8; i++) {
				resourcesDisplay.Add (transform.GetChild (0).GetChild (i).gameObject.GetComponent<Text> ());
				resourcesOffered.Add (0);
				resourcesOfferedDisplay.Add (transform.GetChild (1).GetChild (i).gameObject.GetComponent<Text> ());
				buttonDesiring.Add (transform.GetChild (2).GetChild (i).gameObject.GetComponent<Button> ());
				buttonOffering.Add (transform.GetChild (3).GetChild (i).gameObject.GetComponent<Button> ());
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
			isSetUp = true;
		}
		for (int i = 0; i < 8; i++) {
			resourcesOffered [i] = 0;
			resourcesOfferedDisplay [i].text = "0";
			resourcesDisplay [i].text = localPlayer.resources [i].ToString();
		}
		discardingSum = 0;
		transform.GetChild (4).gameObject.GetComponent<Button> ().interactable = false;
		transform.GetChild (8).gameObject.GetComponent<Text> ().text = (resourceSumRequired - discardingSum).ToString () + " more to discard.";
	}

	private void ModifyOffer(int resourceIndex, int change){
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		if ((discardingSum+change <= resourceSumRequired) && (resourcesOffered [resourceIndex] + change >= 0) && (localPlayer.resources [resourceIndex] >= (resourcesOffered [resourceIndex] + change))) {
			resourcesOffered [resourceIndex] += change;
			resourcesOfferedDisplay[resourceIndex].text = resourcesOffered [resourceIndex].ToString ();
			resourcesDisplay [resourceIndex].text = (localPlayer.resources [resourceIndex] - resourcesOffered [resourceIndex]).ToString ();
			discardingSum += change;
			transform.GetChild (4).gameObject.GetComponent<Button> ().interactable = (discardingSum == resourceSumRequired);
			transform.GetChild (8).gameObject.GetComponent<Text> ().text = (resourceSumRequired - discardingSum).ToString () + " more to discard.";
		}
	}

	public void RequestDiscardResourceToBank(){
		gameObject.SetActive (true);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		transform.GetChild (4).gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.GetChild (4).gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
			localPlayer.processTrade(resourcesOffered);
			localPlayer.CmdResponseToServer();
		});
	}

	public void RequestDiscardResourceToPlayer(int targetPlayerIndex){
		gameObject.SetActive (true);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();
		transform.GetChild (4).gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
		transform.GetChild (4).gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
			localPlayer.CmdAcceptTradeOffer(resourcesOffered.ToArray(), targetPlayerIndex);
		});
	}
}
