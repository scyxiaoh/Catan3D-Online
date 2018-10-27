using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.NetworkLobby
{
	public class LobbyPanelCreateGame : MonoBehaviour {

		public LobbyManager lobbyManager;

		public Button cnkSelection;
		public Button seafareSelection;
		public Button submit;
		public Button mapSelectionL;
		public Button mapSelectionR;
		public Button vpSelectionL;
		public Button vpSelectionR;
		public Text textCnkSelection;
		public Text textSeafareSelection;
		public Text textVP;

		public InputField matchNameInput;

		bool CnkSelected = false;
		bool SeafareSelected = false;
		public int vp = 10;

		void Start () {

			cnkSelection.onClick.AddListener(bCnkOnClick);
			seafareSelection.onClick.AddListener(bSeafareOnClick);
			submit.onClick.AddListener(bSubmitOnClick);
			mapSelectionL.onClick.AddListener (switchMapLeft);
			mapSelectionR.onClick.AddListener (switchMapRight);
			vpSelectionL.onClick.AddListener (decrementVP);
			vpSelectionR.onClick.AddListener (incrementVP);
		}

		void bCnkOnClick(){
			SpriteState newST = new SpriteState ();
			if (CnkSelected) {
				newST.highlightedSprite = Resources.Load<Sprite> ("UI/button/button_unselected_highlighted");
				newST.disabledSprite = Resources.Load<Sprite> ("UI/button/button_unselected_disabled");
				cnkSelection.GetComponent<Image>().sprite = Resources.Load<Sprite> ("UI/button/button_unselected_normal");
				CnkSelected = false;
				textCnkSelection.text = "City&Knights off";
				vp = 10;
				textVP.text = vp.ToString();
			} else {
				newST.pressedSprite = Resources.Load<Sprite> ("UI/button/button_selected_pressed");
				newST.highlightedSprite = Resources.Load<Sprite> ("UI/button/button_selected_highlighted");
				newST.disabledSprite = Resources.Load<Sprite> ("UI/button/button_selected_disabled");
				cnkSelection.GetComponent<Image>().sprite = Resources.Load<Sprite> ("UI/button/button_selected_normal");
				CnkSelected = true;
				textCnkSelection.text = "City&Knights on";
				vp = 13;
				textVP.text = vp.ToString();
			}
			cnkSelection.spriteState = newST;
		}

		void bSeafareOnClick(){
			SpriteState newST = new SpriteState ();
			if (SeafareSelected) {
				newST.highlightedSprite = Resources.Load<Sprite> ("UI/button/button_unselected_highlighted");
				newST.disabledSprite = Resources.Load<Sprite> ("UI/button/button_unselected_disabled");
				seafareSelection.GetComponent<Image>().sprite = Resources.Load<Sprite> ("UI/button/button_unselected_normal");
				SeafareSelected = false;
				textSeafareSelection.text = "Seafare off";
			} else {
				newST.pressedSprite = Resources.Load<Sprite> ("UI/button/button_selected_pressed");
				newST.highlightedSprite = Resources.Load<Sprite> ("UI/button/button_selected_highlighted");
				newST.disabledSprite = Resources.Load<Sprite> ("UI/button/button_selected_disabled");
				seafareSelection.GetComponent<Image>().sprite = Resources.Load<Sprite> ("UI/button/button_selected_normal");
				SeafareSelected = true;
				textSeafareSelection.text = "Seafare on";
			}
			seafareSelection.spriteState = newST;
		}

		void bSubmitOnClick(){
			lobbyManager.StartMatchMaker();
			lobbyManager.matchMaker.CreateMatch(
				matchNameInput.text,
				(uint)lobbyManager.maxPlayers,
				true,
				"", "", "", 0, 0,
				lobbyManager.OnMatchCreate);

			lobbyManager.backDelegate = lobbyManager.StopHost;
			lobbyManager._isMatchmaking = true;
			lobbyManager.lobbyPanel.GetComponent<LobbyPlayerList> ().ChangeTitle (matchNameInput.text);
			lobbyManager.DisplayIsConnecting();

		}

		void switchMapLeft(){
			//TODO
		}

		void switchMapRight(){
			//TODO
		}

		void decrementVP() {
			int min;
			if (CnkSelected) {
				min = 10;
			} else {
				min = 7;
			}
			if (vp != min) {
				vp--;
				textVP.text = vp.ToString();
			}
		}

		void incrementVP(){
			int max;
			if (CnkSelected) {
				max = 16;
			} else {
				max = 13;
			}
			if (vp != max) {
				vp++;
				textVP.text = vp.ToString();
			}
		}
	}
}
