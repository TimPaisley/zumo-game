using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Zumo {
    public class Deathmatch : MonoBehaviour {
        //TODO reposition the boards in the editor so this isn't necessary
        static readonly Vector3 BOARD_OFFSET = new Vector3(0, 1f, 0);

		public Camera sceneCamera;
		public Countdown countdown;
        public HUD hud;
        public PickupSpawner spawner;
        
        GameManager gm;
        AudioSource introPlayer;

		Board board;
		List<Animal> playerAnimals = new List<Animal>();

		bool gameOver = false;

		void Awake () {
			gm = FindObjectOfType<GameManager>();
            introPlayer = gameObject.AddComponent<AudioSource>();

			sceneCamera.gameObject.SetActive(false);
		}

		void Start () {
            gm.musicManager.Stop();

            setupBoard();
			setupPlayerAnimals();
            hud.Setup(playerAnimals);
            spawner.Enable(board.pickupSpawnPoints);

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
			var spawnPoints = board.animalSpawnPoints.AsEnumerable().GetEnumerator();

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
            gm.cameraManager.Use(sceneCamera, 1f);
            yield return new WaitForSeconds(1f);

            yield return countdown.Play();

			foreach (var animal in playerAnimals) {
				animal.isActive = true;
			}

            introPlayer.PlayOneShot(board.intro);
            yield return new WaitForSeconds(board.intro.length);

            countdown.Stop();

            gm.musicManager.Play(board.music);
		}

		void showWinner (Animal winner) {
			Debug.Log(winner.player.name + " Wins!");
		}
    }
}
