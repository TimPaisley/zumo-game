using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {
	public float bounceForce = 10.0f;
    public bool instantPlay = false;

	public VirtualScene splashScene;
    public VirtualScene readyUpScene;
    public VirtualScene characterChoiceScene;
    public VirtualScene boardChoiceScene;
    public VirtualScene inGameScene;

    public BoardController currentBoard;

	public ParticleSystem collisionPS;
	private ParticleSystem.EmissionModule collisionEM;

    void Start () {
        if (instantPlay) {
            setupInstantPlay();
            inGameScene.Activate();
		} else {
            inGameScene.Deactivate();
            characterChoiceScene.Deactivate();
			readyUpScene.Deactivate();
            boardChoiceScene.Deactivate();
            
			splashScene.Activate();
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

	public void ApplyBombForce (Vector3 pos, float pow) {
		AnimalController[] animals = FindObjectsOfType<AnimalController> ();

		foreach (AnimalController a in animals) {
            if (!a.foxAbility)
            {
                Vector3 awayFromBomb = (a.transform.position - pos);
                a.rb.AddForce((awayFromBomb.normalized + new Vector3(0, 1, 0)) * (pow / awayFromBomb.magnitude * 1.5f), ForceMode.Impulse);
                Debug.Log((awayFromBomb.normalized + new Vector3(0, 1, 0)) * (1 / awayFromBomb.magnitude));
            }
		}
	}

	public void ShowCollisionParticle (Vector3 pos) {
        Debug.Log ("Show Collision Particle");
        collisionPS.transform.position = pos;
        collisionEM.enabled = true;
        collisionPS.Simulate(0.0f,true,true);
    }
}
