﻿using UnityEngine;
using System.Collections;

public class DeathmatchScene : VirtualScene {
    public Canvas hudCanvas;
    public GameObject winText;
    public FollowAnimal basePlayerIndicator;
    public Transform[] spawnPoints;

    private CameraManager cameraManager;
    private MusicManager musicManager;
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

        musicManager = FindObjectOfType<MusicManager>();
        cameraManager = FindObjectOfType<CameraManager>();
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
            musicManager.Play(musicManager.winSong);

            gameOver = true;
        }
    }

    public void Prepare (PlayerController[] readyPlayers) {
        gameStarted = false;
        players = readyPlayers;

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
        }

        cameraManager.Use(cameraManager.gameCamera);
    }

    public override void Activate () {
        base.Activate();

        musicManager.Play(musicManager.gameSong);

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
