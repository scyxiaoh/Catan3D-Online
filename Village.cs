using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Village : MonoBehaviour {

	public Player owner;
	public VillageType vt;
	public bool hasCityWall;
	public int vertexIndex;

	void OnMouseEnter(){
		gameObject.transform.localScale = new Vector3 (1.2f, 1.2f, 1.2f);
	}

	void OnMouseExit(){
		gameObject.transform.localScale = new Vector3 (1.0f, 1.0f, 1.0f);
	}

	void OnMouseUp(){
		Vertex myVertex = gameObject.transform.parent.gameObject.GetComponent<Vertex>();
		//disable all villages or knights selections
		myVertex.gameBoard.panelActions.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Button>().onClick.Invoke();
		myVertex.gameBoard.panelActions.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Button>().onClick.Invoke();
		myVertex.gameBoard.panelActions.transform.GetChild(5).GetChild(4).gameObject.GetComponent<Button>().onClick.Invoke();
		myVertex.gameObject.transform.GetChild (0).gameObject.SetActive (true);		//show the selection mark (arrow)
		if (vt == VillageType.Settlement) {
			for (int i = 1; i < 6; i++) {
				if (i == 3) {
					myVertex.gameBoard.panelActions.transform.GetChild (i).gameObject.SetActive (true);
				} else {
					myVertex.gameBoard.panelActions.transform.GetChild (i).gameObject.SetActive (false);
				}
			}
			myVertex.gameBoard.panelActions.transform.GetChild(3).GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
			myVertex.gameBoard.panelActions.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
			myVertex.gameBoard.panelActions.transform.GetChild(3).GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(myVertex.UpgradeSettlement);
			myVertex.gameBoard.panelActions.transform.GetChild(3).GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(delegate {
				myVertex.gameBoard.panelActions.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Button>().onClick.Invoke();
			});
			myVertex.gameBoard.panelActions.transform.GetChild(3).GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(delegate{
				myVertex.gameObject.transform.GetChild (0).gameObject.SetActive (false);
			});
		}
		if ((int)vt > 0) {
			for (int i = 1; i < 6; i++) {
				if (i == 4) {
					myVertex.gameBoard.panelActions.transform.GetChild (i).gameObject.SetActive (true);
				} else {
					myVertex.gameBoard.panelActions.transform.GetChild (i).gameObject.SetActive (false);
				}
			}
			myVertex.gameBoard.panelActions.transform.GetChild (4).GetChild (0).gameObject.GetComponent<Button> ().interactable = !hasCityWall;
			myVertex.gameBoard.panelActions.transform.GetChild(4).GetChild(0).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
			myVertex.gameBoard.panelActions.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
			myVertex.gameBoard.panelActions.transform.GetChild(4).GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(myVertex.BuildCityWall);
			myVertex.gameBoard.panelActions.transform.GetChild(4).GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(delegate {
				myVertex.gameBoard.panelActions.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Button>().onClick.Invoke();
			});
			myVertex.gameBoard.panelActions.transform.GetChild(4).GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(delegate{
				myVertex.gameObject.transform.GetChild (0).gameObject.SetActive (false);
			});

		}

	}
}
