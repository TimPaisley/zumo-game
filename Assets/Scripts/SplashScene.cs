using UnityEngine;
using InControl;
using System.Collections;
using System.Linq;

public class SplashScene : VirtualScene {
	public AudioSource menuSwitchSound;
    public Canvas splashCanvas;

	private MusicManager musicManager;
	private CameraManager cameraManager;
	private GameManager gameManager;

	void Awake () {
		musicManager = FindObjectOfType<MusicManager>();
		cameraManager = FindObjectOfType<CameraManager>();
		gameManager = FindObjectOfType<GameManager>();

        splashCanvas.enabled = false;
	}

	void Update () {
		if (InputManager.Devices.Any(device => device.Action1.IsPressed) || Input.GetKeyDown(KeyCode.Return)) {
			menuSwitchSound.PlayOneShot(menuSwitchSound.clip);

			gameManager.readyUpScene.Prepare(null);
			gameManager.readyUpScene.Activate();
			Deactivate();
		}

        cameraManager.mainCamera.transform.position = cameraManager.splashCamera.transform.position +
            Vector3.up * (Mathf.Sin(Time.fixedTime) * 0.5f);
	}

    public override void Activate () {
        cameraManager.mainCamera.gameObject.SetActive(false);
        cameraManager.blackoutCamera.gameObject.SetActive(true);

        StartCoroutine(showScene());
    }

    private IEnumerator showScene () {
        yield return new WaitForSeconds(0.3f);

        cameraManager.mainCamera.gameObject.SetActive(true);
        cameraManager.blackoutCamera.gameObject.SetActive(false);

        cameraManager.Use(cameraManager.splashCamera);
		musicManager.Play(musicManager.menuSong);

        splashCanvas.enabled = true;
	}
}
