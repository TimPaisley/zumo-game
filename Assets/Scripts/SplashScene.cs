using UnityEngine;
using InControl;
using System.Collections;
using System.Linq;

public class SplashScene : VirtualScene {
	private CameraManager cameraManager;
	private GameManager gameManager;

	void Awake () {
		cameraManager = FindObjectOfType<CameraManager>();
		gameManager = FindObjectOfType<GameManager>();
	}

	void Update () {
		if (InputManager.Devices.Any(device => device.Action1.IsPressed) || Input.GetKeyDown(KeyCode.Return)) {
			gameManager.readyUpScene.Prepare(null);
			gameManager.readyUpScene.Activate();
			Deactivate();
		}
	}

	public override void Activate () {
		cameraManager.Use(cameraManager.splashCamera);
	}
}
