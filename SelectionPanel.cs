using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectionPanel : MonoBehaviour {

	public GameObject panelSelectImprovement;
	public Button[] buttonImprovements = new Button[3];
	public GameObject[] imgImprovementSelections = new GameObject[3];

	public GameObject panelSelectResource;
	public Button[] buttonResources = new Button[5];
	public GameObject[] imgResourceSelections = new GameObject[5];

	public GameObject panelSelectCommodity;
	public Button[] buttonCommodities = new Button[3];
	public GameObject[] imgCommoditySelections = new GameObject[3];

	public GameObject panelSelectDice;
	public Button buttonWDiceL;
	public Button buttonWDiceR;
	public Button buttonRDiceL;
	public Button buttonRDiceR;
	public Image imgWhiteDice;
	public Image imgRedDice;
	public int whiteDiceSelection;
	public int redDiceSelection;


	public delegate void SelectionDelegate();
	public SelectionDelegate waitingDelegate;
	public void RunWaitingDelegate(){
		if (waitingDelegate != null) {
			waitingDelegate ();
			waitingDelegate = null;
		}
	}

	public void RequestImprovementSelection(bool canTrade, bool canPolitics, bool canScience){
		panelSelectResource.SetActive (false);
		panelSelectDice.SetActive (false);
		panelSelectCommodity.SetActive (false);

		ImprovementClearSelection ();
		for (int i = 0; i < 3; i++) {
			buttonImprovements [i].interactable = true;
			buttonImprovements [i].onClick.RemoveAllListeners ();
		}
		if (canTrade) {
			buttonImprovements [0].onClick.AddListener (delegate {
				ImprovementClearSelection ();
				imgImprovementSelections [0].SetActive (true);
				waitingDelegate = delegate {
					GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdRequestDrawProgressCard (0);
				};
			});
		} else {
			buttonImprovements [0].interactable = false;
		}

		if (canPolitics) {
			buttonImprovements [1].onClick.AddListener (delegate {
				ImprovementClearSelection ();
				imgImprovementSelections [1].SetActive (true);
				waitingDelegate = delegate {
					GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdRequestDrawProgressCard (1);
				};
			});
		} else {
			buttonImprovements [1].interactable = false;
		}

		if (canScience) {
			buttonImprovements [2].onClick.AddListener (delegate {
				ImprovementClearSelection ();
				imgImprovementSelections [2].SetActive (true);
				waitingDelegate = delegate {
					GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdRequestDrawProgressCard (2);
				};
			});
		} else {
			buttonImprovements [2].interactable = false;
		}


		panelSelectImprovement.SetActive (true);
		gameObject.SetActive (true);


	}

	public void RequestResourceSelection (){
		panelSelectImprovement.SetActive (false);
		panelSelectDice.SetActive (false);
		panelSelectCommodity.SetActive (false);

		ResourceClearSelection ();
		for (int i = 0; i < 5; i++) {
			buttonResources [i].onClick.RemoveAllListeners ();
		}

		buttonResources [0].onClick.AddListener (delegate {
			ResourceClearSelection ();
			imgResourceSelections [0].SetActive (true);
			waitingDelegate = delegate {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().OnResourcesChanged(ResourceType.Lumber,1);
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdResponseToServer();
			};
		});
		buttonResources [1].onClick.AddListener (delegate {
			ResourceClearSelection ();
			imgResourceSelections [1].SetActive (true);
			waitingDelegate = delegate {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().OnResourcesChanged(ResourceType.Brick,1);
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdResponseToServer();
			};
		});

		buttonResources [2].onClick.AddListener (delegate {
			ResourceClearSelection ();
			imgResourceSelections [2].SetActive (true);
			waitingDelegate = delegate {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().OnResourcesChanged(ResourceType.Wool,1);
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdResponseToServer();
			};
		});

		buttonResources [3].onClick.AddListener (delegate {
			ResourceClearSelection ();
			imgResourceSelections [3].SetActive (true);
			waitingDelegate = delegate {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().OnResourcesChanged(ResourceType.Grain,1);
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdResponseToServer();
			};
		});

		buttonResources [4].onClick.AddListener (delegate {
			ResourceClearSelection ();
			imgResourceSelections [4].SetActive (true);
			waitingDelegate = delegate {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().OnResourcesChanged(ResourceType.Ore,1);
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdResponseToServer();
			};
		});

		panelSelectResource.SetActive (true);
		gameObject.SetActive (true);
		
	}

	public void RequestResourceMonopoly(){
		panelSelectImprovement.SetActive (false);
		panelSelectDice.SetActive (false);
		panelSelectCommodity.SetActive (false);

		ResourceClearSelection ();
		for (int i = 0; i < 5; i++) {
			buttonResources [i].onClick.RemoveAllListeners ();
		}

		buttonResources [0].onClick.AddListener (delegate {
			ResourceClearSelection ();
			imgResourceSelections [0].SetActive (true);
			waitingDelegate = delegate {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdPlayedResourceMonopoly(0);
			};
		});
		buttonResources [1].onClick.AddListener (delegate {
			ResourceClearSelection ();
			imgResourceSelections [1].SetActive (true);
			waitingDelegate = delegate {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdPlayedResourceMonopoly(1);
			};
		});

		buttonResources [2].onClick.AddListener (delegate {
			ResourceClearSelection ();
			imgResourceSelections [2].SetActive (true);
			waitingDelegate = delegate {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdPlayedResourceMonopoly(2);
			};
		});

		buttonResources [3].onClick.AddListener (delegate {
			ResourceClearSelection ();
			imgResourceSelections [3].SetActive (true);
			waitingDelegate = delegate {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdPlayedResourceMonopoly(3);
			};
		});

		buttonResources [4].onClick.AddListener (delegate {
			ResourceClearSelection ();
			imgResourceSelections [4].SetActive (true);
			waitingDelegate = delegate {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdPlayedResourceMonopoly(4);
			};
		});

		panelSelectResource.SetActive (true);
		gameObject.SetActive (true);
	}

	public void RequestTradeMonopoly(){
		panelSelectImprovement.SetActive (false);
		panelSelectResource.SetActive (false);
		panelSelectDice.SetActive (false);

		CommodityClearSelection ();
		for (int i = 0; i < 3; i++) {
			buttonCommodities [i].onClick.RemoveAllListeners ();
		}

		buttonCommodities [0].onClick.AddListener (delegate {
			CommodityClearSelection();
			imgCommoditySelections[0].SetActive(true);
			waitingDelegate = delegate() {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdPlayedTradeMonopoly(5);
			};
		});

		buttonCommodities [1].onClick.AddListener (delegate {
			CommodityClearSelection();
			imgCommoditySelections[1].SetActive(true);
			waitingDelegate = delegate() {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdPlayedTradeMonopoly(6);
			};
		});

		buttonCommodities [2].onClick.AddListener (delegate {
			CommodityClearSelection();
			imgCommoditySelections[2].SetActive(true);
			waitingDelegate = delegate() {
				GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdPlayedTradeMonopoly(7);
			};
		});


		panelSelectCommodity.SetActive (true);
		gameObject.SetActive (true);

	}

	public void RequestDiceSelection(){
		panelSelectImprovement.SetActive (false);
		panelSelectResource.SetActive (false);
		panelSelectCommodity.SetActive (false);

		whiteDiceSelection = 1;
		redDiceSelection = 1;
		imgWhiteDice.sprite = Resources.Load<Sprite> ("UI/dice/dices_white_1");
		imgRedDice.sprite = Resources.Load<Sprite> ("UI/dice/dices_red_1");
		waitingDelegate = delegate {
			GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdSetDice(whiteDiceSelection, redDiceSelection);
			GameObject.Find ("Local Player Panel").GetComponent<Player> ().CmdResponseToServer();
		};
		panelSelectDice.SetActive (true);
		gameObject.SetActive (true);
	}

	public void WhiteDiceSwap(int change){
		if ((change < 0 && whiteDiceSelection > 1) || (change > 0 && whiteDiceSelection < 6)) {
			whiteDiceSelection += change;
			Debug.Log (whiteDiceSelection);
			imgWhiteDice.sprite = Resources.Load<Sprite> ("UI/dice/dices_white_" + whiteDiceSelection.ToString());
		}
	}

	public void RedDiceSwap(int change){
		if ((change < 0 && redDiceSelection > 1) || (change > 0 && redDiceSelection < 6)) {
			redDiceSelection += change;
			Debug.Log (redDiceSelection);
			imgRedDice.sprite = Resources.Load<Sprite> ("UI/dice/dices_red_" + redDiceSelection.ToString());
		}
	}

	public void ImprovementClearSelection(){
		foreach (GameObject o in imgImprovementSelections) {
			o.SetActive (false);
		}
	}

	public void ResourceClearSelection(){
		foreach (GameObject o in imgResourceSelections) {
			o.SetActive (false);
		}
	}

	public void CommodityClearSelection(){
		foreach (GameObject o in imgCommoditySelections) {
			o.SetActive (false);
		}
	}
		
}
