using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.NetworkLobby
{
	public class LobbyPanelMenu : MonoBehaviour {

		public LobbyManager lobbyManager;

		public Button buttonCreateGame;
		public Button buttonJoinGame;
		public Button buttonExit;

		bool cnkSelected, seafareSelected = false;

		void Start () {
			buttonCreateGame.onClick.AddListener (OnClickCreateGame);
			buttonJoinGame.onClick.AddListener (OnClickJoinGame);
			buttonExit.onClick.AddListener(OnClickExitGame);
		}
		public void OnClickCreateGame(){
			lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
			lobbyManager.ChangeTo(lobbyManager.createGamePanel);
		}
		public void OnClickJoinGame(){
			lobbyManager.StartMatchMaker();
			lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
			lobbyManager.ChangeTo(lobbyManager.joinGamePanel);
		}

		void OnClickExitGame(){
			Application.Quit();
		}
	}
}
