using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	public float bounceForce = 10.0f;
    public GameObject winText;

    public AudioSource gameMusic;
    public AudioSource winMusic;

	private PlayerController[] players;
    private delegate bool PlayerChecker (PlayerController player);

    private bool gameOver = false;

	void Start () {
		players = FindObjectsOfType<PlayerController> ();

        winText.SetActive(false);
	}

    void Update () {
        if (gameOver) {
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

    private int countPlayers(PlayerChecker checker) {
        var count = 0;

        foreach (var player in players) {
            if (checker(player)) count++;
        }

        return count;
    }

    private PlayerController findPlayer(PlayerChecker checker) {
        foreach (var player in players) {
            if (checker(player)) return player;
        }

        return null;
    }
}
