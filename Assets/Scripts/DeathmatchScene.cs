using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class DeathmatchScene : VirtualScene {
	[Header("UI")]
    public Canvas hudCanvas;
	public Canvas pauseMenu;
	public Text pauseTitleText;
	public Text pauseActionText;
	public Text pauseResetText;

	[Header("Countdown")]
    public GameObject[] countdownItems;
    public AudioSource countdownNumberSound;
    public AudioSource countdownCompleteSound;

	[Header("Gameplay")]
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
		pauseMenu.gameObject.SetActive(false);
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
            if (players.Any(player => player.input.actionButton.IsPressed)) {
                rematch();
            } else if (players.Any(player => player.input.menuButton.WasPressed)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            return;
        }
        
        var alivePlayers = players.Count(player => player.isAlive);

        if (alivePlayers == 1) {
            var winningPlayer = players.First(player => player.isAlive);

//            foreach (var text in winText.GetComponentsInChildren<TextMesh>()) {
//                text.text = "Player " + (winningPlayer.playerIndex + 1) + " Wins!";
//            }

			pauseMenu.gameObject.SetActive(true);
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
        gameOver = false;
        players = readyPlayers;
        playerIndicators = new FollowAnimal[readyPlayers.Length];

		pauseMenu.gameObject.SetActive(false);
        basePlayerIndicator.gameObject.SetActive(false);

        foreach (var player in players) {
            player.isAlive = false;
        }

        for (var i = 0; i < players.Length; i++) {
			players[i].ResetAnimal(spawnPoints[i]);

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

    private void rematch () {
        foreach (var indicator in playerIndicators) {
			Destroy(indicator.gameObject);
        }

        Prepare(players);

        Activate();
    }
}
