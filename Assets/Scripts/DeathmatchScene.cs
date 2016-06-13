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
	public AudioSource pauseSound;

    [Header("Celebration")]
    public GameObject celebrationContainer;
    public ParticleSystem playerCelebrationParticles;

    private GameManager gameManager;
    private CameraManager cameraManager;
    private MusicManager musicManager;
    private MenuBackgroundManager menuBackgroundManager;
    private PlayerController[] players;
    private FollowAnimal[] playerIndicators;
    private delegate bool PlayerChecker (PlayerController player);

	private Color pausedTextColor;

    public bool gameStarted = false;
    private bool gameOver = false;

    public bool inProgress {
        get { return gameStarted && !gameOver; }
    }

	private bool isPaused {
		get { return Time.timeScale == 0; }
	}
    
    void Awake () {
		pauseMenu.gameObject.SetActive(false);
        basePlayerIndicator.gameObject.SetActive(false);
        celebrationContainer.gameObject.SetActive(false);

        foreach (var item in countdownItems) {
            item.SetActive(false);
        }

        gameManager = FindObjectOfType<GameManager>();
        musicManager = FindObjectOfType<MusicManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        menuBackgroundManager = FindObjectOfType<MenuBackgroundManager>();

		pausedTextColor = pauseTitleText.color;
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

			pauseTitleText.text = winningPlayer.playerName + " Wins!";
			pauseTitleText.color = winningPlayer.color;
			pauseActionText.text = "Rematch";

			pauseMenu.gameObject.SetActive(true);
            musicManager.Play(musicManager.winSong);

            celebrationContainer.SetActive(true);
            playerCelebrationParticles.GetComponent<ParticleSystemRenderer>().material.color = winningPlayer.color;

            gameOver = true;
        }

		if (isPaused) {
			if (players.Any(player => (player.input.menuButton.WasPressed || player.input.actionButton.WasPressed))) {
				// Unpause
				Time.timeScale = 1;
				pauseSound.PlayOneShot(pauseSound.clip);

				pauseMenu.gameObject.SetActive(false);
			} else if (players.Any(player => player.input.backButton.WasPressed)) {
				// Go to menu
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}
		} else if (players.Any(player => player.input.menuButton.WasPressed)) {
			// Pause
			Time.timeScale = 0;
			pauseSound.PlayOneShot(pauseSound.clip);

			pauseTitleText.text = "Paused";
			pauseTitleText.color = pausedTextColor;
			pauseActionText.text = "Resume";
			pauseMenu.gameObject.SetActive(true);
		}
    }

    public override void Prepare (PlayerController[] readyPlayers) {
        gameStarted = false;
        gameOver = false;
        players = readyPlayers;
        playerIndicators = new FollowAnimal[readyPlayers.Length];

		pauseMenu.gameObject.SetActive(false);
        basePlayerIndicator.gameObject.SetActive(false);
		celebrationContainer.gameObject.SetActive(false);

        foreach (var player in players) {
            player.isAlive = false;
        }

        for (var i = 0; i < players.Length; i++) {
			players[i].ResetAnimal(gameManager.currentBoard.spawnPoints[i]);

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
