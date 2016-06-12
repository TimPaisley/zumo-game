using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {
	public float bounceForce = 10.0f;
    public bool instantPlay = false;

    public VirtualScene readyUpScene;
    public VirtualScene characterChoiceScene;
    public VirtualScene boardChoiceScene;
    public VirtualScene inGameScene;

	public ParticleSystem collisionPS;
	private ParticleSystem.EmissionModule collisionEM;

    void Start () {
        if (instantPlay) {
            setupInstantPlay();
            inGameScene.Activate();
        } else {
            inGameScene.Deactivate();
            characterChoiceScene.Deactivate();
            if (boardChoiceScene) boardChoiceScene.Deactivate(); //TODO board choice
            readyUpScene.Activate();
        }

		collisionEM = collisionPS.emission;
	}

    private void setupInstantPlay() {
        var players = FindObjectsOfType<PlayerController>().OrderBy(player => player.playerIndex).ToArray();

        foreach (var player in players) {
            player.baseAnimal.gameObject.SetActive(false);
            player.Setup(player.playerIndex);
			player.ResetAnimal(player.baseAnimal.transform);
            player.isReady = true;
        }

        inGameScene.Prepare(players);
    }

	public void ShowCollisionParticle (Vector3 pos) {
        Debug.Log ("Show Collision Particle");
        collisionPS.transform.position = pos;
        collisionEM.enabled = true;
        collisionPS.Simulate(0.0f,true,true);
    }
}
