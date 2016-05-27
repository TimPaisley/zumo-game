using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public float bounceForce = 10.0f;

    public ReadyUpScene readyUpScene;
    public DeathmatchScene inGameScene;
    
    void Start () {
        inGameScene.Deactivate();
        readyUpScene.Activate();
	}
}
