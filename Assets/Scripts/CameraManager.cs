using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
    // Stores the names of the cameras in defaultCameras. Make sure you update
    // this if you change defaultCameras!
    public enum CameraPosition {
        Menu,
        Game
    }

    [Header("Update the CameraManager class if you change this!")]
    public Camera[] defaultCameras;

    private Camera mainCamera;

	void Start () {
        mainCamera = Instantiate(defaultCameras[0]);
        
        foreach (var camera in defaultCameras) {
            camera.gameObject.SetActive(false);
        }
	}
	
	void Update () {
	
	}

    public void ChangePosition (CameraPosition position) {
        var camera = defaultCameras[(int)position];

        mainCamera.transform.position = camera.transform.position;
        mainCamera.transform.localRotation = camera.transform.localRotation;
    }
}
