using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Zumo {
	class ChoosableBoard : MonoBehaviour {
		public Image selectionIndicator;
		public Board board;

		Vector2 basePosition;
		HashSet<Player> playerVotes;

		public int votes {
			get { return playerVotes.Count; }
		}

		public float indicatorAngle {
			get { return selectionIndicator.transform.localEulerAngles.z; }
		}

		void Awake () {
			board.transform.SetParent(null);
			DontDestroyOnLoad(board.gameObject);

			board.gameObject.SetActive(false);
			selectionIndicator.gameObject.SetActive(false);

			playerVotes = new HashSet<Player>();

			basePosition = new Vector2(
				-Mathf.Sin(indicatorAngle * Mathf.Deg2Rad),
				Mathf.Cos(indicatorAngle * Mathf.Deg2Rad)
			);
		}

		public void Vote (Player player) {
			playerVotes.Add(player);

			selectionIndicator.gameObject.SetActive(true);
		}

		public void RemoveVote (Player player) {
			playerVotes.Remove(player);

			if (playerVotes.Count == 0) {
				selectionIndicator.gameObject.SetActive(false);
			}
		}

		public bool VotedBy (Player player) {
			return playerVotes.Contains(player);
		}

		public float DistanceFrom (Vector2 point) {
			return Vector2.Distance(point, basePosition);
		}
	}
}
