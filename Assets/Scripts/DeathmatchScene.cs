using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class DeathmatchScene : VirtualScene {
    public Canvas hudCanvas;
    public GameObject winText;
    public GameObject[] countdownItems;
    public AudioSource countdownNumberSound;
    public AudioSource countdownCompleteSound;
    public FollowAnimal basePlayerIndicator;
    public Transform[] spawnPoints;

    private CameraManager cameraManager;
    private MusicManager musicManager;
    private MenuBackgroundManager menuBackgroundManager;
    private PlayerController[] players;
    private FollowAnimal[] playerIndicators;
    private delegate bool PlayerChecker (PlayerController player);

    public bool gameStarted = false;
    private bool gameOver = false;

    public bool inProgress {
        get { return gameStarted && !gameOver; }
    }
    
    void Awake () {
        winText.SetActive(false);
        basePlayerIndicator.gameObject.SetActive(false);

        foreach (var item in countdownItems) {
            item.SetActive(false);
        }

        musicManager = FindObjectOfType<MusicManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        menuBackgroundManager = FindObjectOfType<MenuBackgroundManager>();
    }
	
	void Update () {
        if (gameOver) {
            //TODO restart the game properly
            if (players.Any(player => player.input.actionButton.IsPressed)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            return;
        }
        
        var alivePlayers = players.Count(player => player.isAlive);

        if (alivePlayers == 1) {
            var winningPlayer = players.First(player => player.isAlive);

            foreach (var text in winText.GetComponentsInChildren<TextMesh>()) {
                text.text = "Player " + (winningPlayer.playerIndex + 1) + " Wins!";
            }

            winText.SetActive(true);
            musicManager.Play(musicManager.winSong);

            gameOver = true;
        }

		foreach (var player in players) {
			if (player.input.menuButton.WasPressed) {
				if (Time.timeScale == 0) {
					Time.timeScale = 1;
					//TODO hide pause menu
				} else {
					Time.timeScale = 0;
					//TODO show pause menu
				}
				return;
			}
		}
    }

    public override void Prepare (PlayerController[] readyPlayers) {
        gameStarted = false;
        players = readyPlayers;
        playerIndicators = new FollowAnimal[readyPlayers.Length];

        winText.SetActive(false);
        basePlayerIndicator.gameObject.SetActive(false);

        for (var i = 0; i < players.Length; i++) {
            var animal = Instantiate(players[i].animal);
            animal.gameObject.SetActive(true);
            animal.transform.position = spawnPoints[i].position;
            animal.transform.localRotation = spawnPoints[i].localRotation;

            players[i].animal = animal;

            var playerIndicator = Instantiate(basePlayerIndicator);
            playerIndicator.player = players[i];
            playerIndicator.transform.SetParent(hudCanvas.transform, false);
            playerIndicator.gameObject.SetActive(true);

            playerIndicators[i] = playerIndicator;
        }

        cameraManager.Use(cameraManager.gameCamera);
        menuBackgroundManager.HideAll();
    }

    public override void Activate () {
        base.Activate();

        musicManager.Stop();

        StartCoroutine(DoCountdown());
    }

    public IEnumerator DoCountdown () {
        for (var i = 0; i < countdownItems.Length - 1; i++) {
            countdownItems[i].SetActive(true);
            countdownNumberSound.PlayOneShot(countdownNumberSound.clip);
            yield return new WaitForSeconds(1);
            countdownItems[i].SetActive(false);
        }

        countdownItems.Last().SetActive(true);
        countdownCompleteSound.PlayOneShot(countdownCompleteSound.clip);

        gameStarted = true;
        foreach (var player in players) {
            player.isAlive = true;
        }
        foreach (var indicator in playerIndicators) {
            indicator.gameStarted = true;
        }

        musicManager.Play(musicManager.gameSong);

        yield return new WaitForSeconds(0.5f);
        countdownItems.Last().SetActive(false);
    }
}
