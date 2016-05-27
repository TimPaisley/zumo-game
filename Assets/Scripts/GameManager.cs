using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public float bounceForce = 10.0f;

    public ReadyUpMenu readyUpScene;
    public DeathmatchManager inGameScene;
    
    void Start () {
        inGameScene.Deactivate();
        readyUpScene.Activate();
	}
}
