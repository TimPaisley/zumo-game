using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zumo {
    public class Deathmatch : MonoBehaviour {
        //TODO reposition the boards in the editor so this isn't necessary
        static readonly Vector3 BOARD_OFFSET = new Vector3(0, 1f, 0);

		public Camera sceneCamera;

		GameManager gm;
		Countdown countdown;
        AudioSource introEffect;

		Board board;
		List<Animal> playerAnimals = new List<Animal>();

		bool gameOver = false;

		void Awake () {
			gm = FindObjectOfType<GameManager>();
			countdown = GetComponent<Countdown>();
            introEffect = gameObject.AddComponent<AudioSource>();

			sceneCamera.gameObject.SetActive(false);
		}

		void Start () {
			gm.cameraManager.Use(sceneCamera);
            gm.musicManager.Stop();

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
			board.transform.position = Vector3.zero + BOARD_OFFSET;
			board.gameObject.SetActive(true);
		}

		void setupPlayerAnimals () {
			var spawnPoints = board.spawnPoints.AsEnumerable().GetEnumerator();

			foreach (var player in gm.state.readyPlayers) {
				var animal = Instantiate(gm.state.chosenAnimals[player]);

				spawnPoints.MoveNext();

				animal.player = player;
				animal.transform.position = spawnPoints.Current.position;
				animal.transform.rotation = spawnPoints.Current.rotation;
				animal.isActive = false;
				animal.gameObject.SetActive(true);

				playerAnimals.Add(animal);
			}
		}

		IEnumerator playCountdownAndBegin () {
			yield return countdown.Play();

			foreach (var animal in playerAnimals) {
				animal.isActive = true;
			}

            introEffect.PlayOneShot(board.intro);
            yield return new WaitForSeconds(board.intro.length);

            gm.musicManager.Play(board.music);
		}

		void showWinner (Animal winner) {
			Debug.Log(winner.player.name + " Wins!");
		}
    }
}
