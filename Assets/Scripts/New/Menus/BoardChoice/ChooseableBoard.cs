using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Zumo {
	class ChooseableBoard : MonoBehaviour {
		public Image selectionIndicator;
		public GameObject board;

		Vector2 basePosition;
		HashSet<PlayerController> playerVotes;

		public int votes {
			get { return playerVotes.Count; }
		}

		void Awake () {
			playerVotes = new HashSet<PlayerController>();

			basePosition = new Vector2(
				-Mathf.Sin(selectionIndicator.transform.localEulerAngles.z * Mathf.Deg2Rad),
				Mathf.Cos(selectionIndicator.transform.localEulerAngles.z * Mathf.Deg2Rad)
			);

			selectionIndicator.gameObject.SetActive(false);
		}

		public void Vote (PlayerController player) {
			playerVotes.Add(player);

			selectionIndicator.gameObject.SetActive(true);
		}

		public void RemoveVote (PlayerController player) {
			playerVotes.Remove(player);

			if (playerVotes.Count == 0) {
				selectionIndicator.gameObject.SetActive(false);
			}
		}

		public bool VotedBy (PlayerController player) {
			return playerVotes.Contains(player);
		}

		public float DistanceFrom (Vector2 point) {
			return Vector2.Distance(point, basePosition);
		}
	}
}
