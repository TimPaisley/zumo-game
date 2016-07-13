using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zumo {
    public class Deathmatch : MonoBehaviour {
		public Countdown countdown;

		GameManager gm;
		Board board;
		List<Animal> playerAnimals = new List<Animal>();

		bool gameOver = false;

		void Awake () {
			gm = FindObjectOfType<GameManager>();
		}

		void Start () {
			Debug.Log("Starting deathmatch");

			setupBoard();
			setupPlayerAnimals();

			StartCoroutine(playCountdownAndBegin());
		}

        void Update () {
			if (!gameOver) {
				var livingAnimals = playerAnimals.Where(animal => animal.isAlive);

				if (livingAnimals.Count() <= 1) {
					gameOver = true;
					showWinner(livingAnimals.FirstOrDefault());
				}
			}
        }

		void setupBoard () {
			board = Instantiate(gm.state.chosenBoard);
			board.transform.position = Vector3.zero;
			board.gameObject.SetActive(true);
		}

		void setupPlayerAnimals () {
			var spawnPoints = board.spawnPoints.AsEnumerable().GetEnumerator();

			foreach (var player in gm.state.readyPlayers) {
				var animal = Instantiate(gm.state.chosenAnimals[player]);

				animal.player = player;
				animal.transform.position = spawnPoints.Current.position;
				animal.transform.rotation = spawnPoints.Current.rotation;
				animal.enabled = false;

				playerAnimals.Add(animal);

				spawnPoints.MoveNext();
			}
		}

		IEnumerator playCountdownAndBegin () {
			yield return countdown.Play();

			foreach (var animal in playerAnimals) {
				animal.enabled = true;
			}
		}

		void showWinner (Animal winner) {
			Debug.Log(winner.player.name + " Wins!");
		}
    }
}
