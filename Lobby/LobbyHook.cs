using UnityEngine;
using UnityEngine.Networking;
using System.Collections;



namespace Prototype.NetworkLobby
{
    // Subclass this and redefine the function you want
    // then add it to the lobby prefab
    public class LobbyHook : MonoBehaviour
    {
		public void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer, int playerCount) { 
			gamePlayer.GetComponent<Player> ().playerName = lobbyPlayer.GetComponent<LobbyPlayer> ().playerName;
			gamePlayer.GetComponent<Player> ().playerColor = lobbyPlayer.GetComponent<LobbyPlayer> ().playerColor;
			if (GameObject.Find ("GameServer") != null) {
				GameObject.Find("GameServer").GetComponent<Game>().Initiate (playerCount, manager.gameObject.GetComponent<LobbyManager>().createGamePanel.gameObject.GetComponent<LobbyPanelCreateGame>().vp);
			}
		}
    }

}
