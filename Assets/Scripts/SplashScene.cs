using UnityEngine;
using InControl;
using System.Collections;
using System.Linq;

public class SplashScene : VirtualScene {
	public AudioSource menuSwitchSound;

	private MusicManager musicManager;
	private CameraManager cameraManager;
	private GameManager gameManager;

	void Awake () {
		musicManager = FindObjectOfType<MusicManager>();
		cameraManager = FindObjectOfType<CameraManager>();
		gameManager = FindObjectOfType<GameManager>();
	}

	void Update () {
		if (InputManager.Devices.Any(device => device.Action1.IsPressed) || Input.GetKeyDown(KeyCode.Return)) {
			menuSwitchSound.PlayOneShot(menuSwitchSound.clip);

			gameManager.readyUpScene.Prepare(null);
			gameManager.readyUpScene.Activate();
			Deactivate();
		}
	}

	public override void Activate () {
		cameraManager.Use(cameraManager.splashCamera);
		musicManager.Play(musicManager.menuSong);
	}
}
