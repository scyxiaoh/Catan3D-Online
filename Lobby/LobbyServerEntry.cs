﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System.Collections;

namespace Prototype.NetworkLobby
{
    public class LobbyServerEntry : MonoBehaviour 
    {
        public Text serverInfoText;
        public Text slotInfo;
        public Button joinButton;

		public void Populate(MatchInfoSnapshot match, LobbyManager lobbyManager)
		{
            serverInfoText.text = match.name;

            slotInfo.text = match.currentSize.ToString() + "/" + match.maxSize.ToString(); ;

            NetworkID networkID = match.networkId;

            joinButton.onClick.RemoveAllListeners();
			joinButton.onClick.AddListener(() => { JoinMatch(networkID, lobbyManager, match.name); });


        }

		void JoinMatch(NetworkID networkID, LobbyManager lobbyManager, string name)
        {
			lobbyManager.matchMaker.JoinMatch(networkID, "", "", "", 0, 0, lobbyManager.OnMatchJoined);
			lobbyManager.backDelegate = lobbyManager.StopClientClbk;
			lobbyManager._isMatchmaking = true;
			lobbyManager.lobbyPanel.GetComponent<LobbyPlayerList> ().ChangeTitle (name);
            lobbyManager.DisplayIsConnecting();
        }

    }
}