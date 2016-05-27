using UnityEngine;
using System.Collections;

public class DeathmatchManager : VirtualScene {
    public GameObject winText;
    public FollowAnimal basePlayerIndicator;

    public AudioSource gameMusic;
    public AudioSource winMusic;
    public AnimalController[] animals;
    public CameraManager cameraManager;

    private PlayerController[] players;
    private delegate bool PlayerChecker (PlayerController player);

    private bool gameStarted = false;
    private bool gameOver = false;

    public bool inProgress {
        get { return gameStarted && !gameOver; }
    }
    
    void Start () {
        winText.SetActive(false);
        basePlayerIndicator.gameObject.SetActive(false);
    }
	
	void Update () {
        if (!inProgress) {
            return;
        }

        var alivePlayers = countPlayers((player) => player.isAlive);

        if (alivePlayers == 1) {
            var winningPlayer = findPlayer((player) => player.isAlive);

            foreach (var text in winText.GetComponentsInChildren<TextMesh>()) {
                text.text = "Player " + (winningPlayer.playerIndex + 1) + " Wins!";
            }

            winText.SetActive(true);
            gameMusic.Stop();
            winMusic.Play();

            gameOver = true;
        }
    }

    public void Prepare (PlayerController[] readyPlayers) {
        gameStarted = false;
        players = readyPlayers;

        winText.SetActive(false);
        basePlayerIndicator.gameObject.SetActive(false);

        foreach (var animal in animals) {
            animal.gameObject.SetActive(false);
        }

        for (var i = 0; i < players.Length; i++) {
            players[i].animal = animals[i];
            players[i].animal.gameObject.SetActive(true);

            var playerIndicator = Instantiate(basePlayerIndicator);
            var canvas = basePlayerIndicator.GetComponentInParent<Canvas>();
            playerIndicator.player = players[i];
            playerIndicator.transform.SetParent(canvas.transform, false);
            playerIndicator.gameObject.SetActive(true);
        }

        cameraManager.ChangePosition(CameraManager.CameraPosition.Game);
    }

    public override void Activate () {
        base.Activate();

        Debug.Log("Activating!");

        gameStarted = true;
        foreach (var player in players) {
            player.isAlive = true;
        }
    }

    private int countPlayers (PlayerChecker checker) {
        var count = 0;

        foreach (var player in players) {
            if (checker(player)) count++;
        }

        return count;
    }

    private PlayerController findPlayer (PlayerChecker checker) {
        foreach (var player in players) {
            if (checker(player)) return player;
        }

        return null;
    }
}
