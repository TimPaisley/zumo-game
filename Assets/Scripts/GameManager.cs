using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour {
	public float bounceForce = 10.0f;
    public bool instantPlay = false;

    public ReadyUpScene readyUpScene;
    public CharacterChoiceScene characterChoiceScene;
    public DeathmatchScene inGameScene;
    
    void Start () {
        if (instantPlay) {
            setupInstantPlay();
            inGameScene.Activate();
        } else {
            inGameScene.Deactivate();
            characterChoiceScene.Deactivate();
            readyUpScene.Activate();
        }
	}

    private void setupInstantPlay() {
        var players = FindObjectsOfType<PlayerController>().OrderBy(player => player.playerIndex).ToArray();

        foreach (var player in players) {
            player.Setup(player.playerIndex);
            player.animal.gameObject.SetActive(false);
        }

        inGameScene.Prepare(players);
    }
}
