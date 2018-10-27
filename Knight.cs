using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Knight : MonoBehaviour {

	public bool isActive;
	public bool hasMovedThisTurn;
	public bool hasPromotedThisTurn;
	public int level;
	public Player owner;
	public int vertexIndex;

	void OnMouseEnter(){
		gameObject.transform.localScale = new Vector3 (1.2f, 1.2f, 1.2f);
	}

	void OnMouseExit(){
		gameObject.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
	}

	void OnMouseUp(){
		Vertex myVertex = gameObject.transform.parent.gameObject.GetComponent<Vertex>();
		Transform panelKnightAction = myVertex.gameBoard.panelActions.transform.GetChild (5);
		Player localPlayer = GameObject.Find ("Local Player Panel").GetComponent<Player> ();

		//disable all villages or knights selections
		myVertex.gameBoard.panelActions.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Button>().onClick.Invoke();
		myVertex.gameBoard.panelActions.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Button>().onClick.Invoke();
		myVertex.gameBoard.panelActions.transform.GetChild(5).GetChild(4).gameObject.GetComponent<Button>().onClick.Invoke();
		myVertex.gameBoard.panelActions.transform.GetChild(6).GetChild(1).gameObject.GetComponent<Button>().onClick.Invoke();
		myVertex.gameObject.transform.GetChild (0).gameObject.SetActive (true);		//show the selection mark (arrow)

		for (int i = 1; i < 7; i++) {
				myVertex.gameBoard.panelActions.transform.GetChild (i).gameObject.SetActive (false);
		}
		panelKnightAction.gameObject.SetActive (true);

		if (isActive) {
			panelKnightAction.GetChild (0).gameObject.GetComponent<Button> ().interactable = false;
			panelKnightAction.GetChild (2).gameObject.GetComponent<Button> ().interactable = true;
			panelKnightAction.GetChild (3).gameObject.GetComponent<Button> ().interactable = !hasMovedThisTurn;
		} else {
			panelKnightAction.GetChild (0).gameObject.GetComponent<Button> ().interactable = true;
			panelKnightAction.GetChild (2).gameObject.GetComponent<Button> ().interactable = false;
			panelKnightAction.GetChild (3).gameObject.GetComponent<Button> ().interactable = false;
		}

		if (owner.fcPoliticsLvl > 3) {
			panelKnightAction.GetChild (1).gameObject.GetComponent<Button> ().interactable = (!hasPromotedThisTurn);
		} else {
			if (level < 3) {
				panelKnightAction.GetChild (1).gameObject.GetComponent<Button> ().interactable = !hasPromotedThisTurn;
			} else {
				//check metropolis
				panelKnightAction.GetChild (1).gameObject.GetComponent<Button> ().interactable = false;
			}
		}


		for (int i = 0; i < panelKnightAction.childCount; i++) {
			panelKnightAction.GetChild (i).gameObject.GetComponent<Button> ().onClick.RemoveAllListeners ();
			if (i < panelKnightAction.childCount - 1) {
				panelKnightAction.GetChild (i).gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
					panelKnightAction.GetChild (panelKnightAction.childCount - 1).gameObject.GetComponent<Button> ().onClick.Invoke ();
				});
			} else {
				panelKnightAction.GetChild (i).gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
					myVertex.gameObject.transform.GetChild(0).gameObject.SetActive(false);
				});
			}
		}
		panelKnightAction.GetChild (0).gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
			myVertex.SetKnightActive(true);
		});

		panelKnightAction.GetChild (1).gameObject.GetComponent<Button> ().onClick.AddListener (delegate {
			myVertex.PromoteKnight(isActive);
		});

		panelKnightAction.GetChild (2).gameObject.GetComponent<Button> ().onClick.AddListener(myVertex.ChaseAwayRobber);

	}
}
