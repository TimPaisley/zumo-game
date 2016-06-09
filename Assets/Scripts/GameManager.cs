using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {
	public float bounceForce = 10.0f;
    public bool instantPlay = false;

    public VirtualScene readyUpScene;
    public VirtualScene characterChoiceScene;
    public VirtualScene inGameScene;

	//public ParticleSystem collisionPS;
	//private ParticleSystem.EmissionModule collisionEM;

	void Awake () {
		readyUpScene.gameObject.SetActive(true);
		characterChoiceScene.gameObject.SetActive(true);
		inGameScene.gameObject.SetActive(true);
	}

    void Start () {
        if (instantPlay) {
            setupInstantPlay();
            inGameScene.Activate();
        } else {
            inGameScene.Deactivate();
            characterChoiceScene.Deactivate();
            readyUpScene.Activate();
        }

		//collisionEM = collisionPS.emission;
	}

    private void setupInstantPlay() {
        var players = FindObjectsOfType<PlayerController>().OrderBy(player => player.playerIndex).ToArray();

        foreach (var player in players) {
            player.animal.gameObject.SetActive(false);
            player.Setup(player.playerIndex);
            player.isReady = true;
        }

        inGameScene.Prepare(players);
    }

	public IEnumerator ShowCollisionParticle (Vector3 pos) {
        //Debug.Log ("Show Collision Particle");
        //collisionPS.transform.position = pos;
        //collisionEM.enabled = true;
        //collisionPS.Simulate(0.0f,true,true);
        //collisionEM.enabled = true;
        //collisionPS.Play ();
        yield return new WaitForSeconds(0.5f);
        //collisionEM.enabled = false;
        //collisionPS.Stop ();
    }
}
