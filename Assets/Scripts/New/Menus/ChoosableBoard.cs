using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Zumo {
	public class ChoosableBoard : MonoBehaviour {
        const float PLAYER_INDICATOR_OFFSET = 9.5f;
        const float INDICATOR_SPACING = 0.5f;

		public Image selectionIndicator;
        public Image basePlayerIndicator;
		public Board board;

		Vector2 basePosition;
		Dictionary<Player, Image> playerVotes;

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
            basePlayerIndicator.gameObject.SetActive(false);

            playerVotes = new Dictionary<Player, Image>();

			basePosition = new Vector2(
				-Mathf.Sin(indicatorAngle * Mathf.Deg2Rad),
				Mathf.Cos(indicatorAngle * Mathf.Deg2Rad)
			);
		}

		public void Vote (Player player) {
			playerVotes.Add(player, createPlayerIndicator(player));

			selectionIndicator.gameObject.SetActive(true);
		}

		public void RemoveVote (Player player) {
            Destroy(playerVotes[player].gameObject);
			playerVotes.Remove(player);

			if (playerVotes.Count == 0) {
				selectionIndicator.gameObject.SetActive(false);
			}
		}

		public bool VotedBy (Player player) {
			return playerVotes.ContainsKey(player);
		}

		public float DistanceFrom (Vector2 point) {
			return Vector2.Distance(point, basePosition);
		}

        public void HideSelectionIndicator () {
            selectionIndicator.gameObject.SetActive(false);
        }

        public void Reset () {
            HideSelectionIndicator();

            foreach (var indicator in playerVotes.Values) {
                Destroy(indicator.gameObject);
            }

            playerVotes.Clear();
        }

        Image createPlayerIndicator(Player player) {
            var indicator = (Image)Instantiate(basePlayerIndicator, basePlayerIndicator.transform.position, basePlayerIndicator.transform.rotation);
            indicator.transform.SetParent(basePlayerIndicator.transform.parent);
            indicator.transform.SetAsFirstSibling();
            
            var centre = basePlayerIndicator.GetComponent<RectTransform>().anchoredPosition;
            var multiplier = PLAYER_INDICATOR_OFFSET + playerVotes.Count * INDICATOR_SPACING;

            indicator.GetComponent<RectTransform>().anchoredPosition = new Vector2(
                centre.x - Mathf.Sin(indicatorAngle * Mathf.Deg2Rad) * multiplier,
                centre.y + Mathf.Cos(indicatorAngle * Mathf.Deg2Rad) * multiplier
            );
            indicator.color = player.color;
            indicator.gameObject.SetActive(true);

            return indicator;
        }
	}
}
