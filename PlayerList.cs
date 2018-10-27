using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


	//List of players in the lobby
	public class PlayerList : MonoBehaviour
	{
		public static PlayerList _instance = null;

		protected VerticalLayoutGroup _layout;
		public List<Player> _players = new List<Player>();

		public void OnEnable()
		{
			_instance = this;
			_layout = gameObject.GetComponent<VerticalLayoutGroup>();
		}

		void Update()
		{
			//this dirty the layout to force it to recompute evryframe (a sync problem between client/server
			//sometime to child being assigned before layout was enabled/init, leading to broken layouting)

			if(_layout)
				_layout.childAlignment = Time.frameCount%2 == 0 ? TextAnchor.UpperCenter : TextAnchor.UpperLeft;
		}

		public void AddPlayer(Player player)
		{
			if (_players.Contains(player))
				return;

			_players.Add(player);

			if (player.isLocalPlayer) {
				player.transform.SetParent(gameObject.transform.parent, false);
			} else {
				player.transform.SetParent(gameObject.transform, false);
			}

		}

		public void RemovePlayer(Player player)
		{
			_players.Remove(player);

		}
	
	}

