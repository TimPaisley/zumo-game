using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
    // Singleton - only one instance of this class may be in a scene
    public static CameraManager instance;

    // Stores the names of the cameras in defaultCameras. Make sure you update
    // this if you change defaultCameras!
    public enum CameraPosition {
        Menu,
        Game
    }

    [Header("Update the CameraManager class if you change this!")]
    public Camera[] defaultCameras;
    public Canvas[] screenCanvases;

    public Camera mainCamera { get; private set; }

    private float movementTime;
    private float movementDuration;
    private Vector3 originalPosition;
    private Vector3 originalLocalEulerAngles;
    private Transform targetTransform;

    void Awake () {
        instance = this;
    }

	void Start () {
        mainCamera = Instantiate(defaultCameras[0]);
        
        foreach (var camera in defaultCameras) {
            camera.gameObject.SetActive(false);
        }
	}
	
	void Update () {
        if (targetTransform) {
            movementTime += Time.deltaTime;

            if (movementTime >= movementDuration) {
                mainCamera.transform.position = targetTransform.position;
                mainCamera.transform.localEulerAngles = targetTransform.localEulerAngles;
                targetTransform = null;
                return;
            }

            var t = movementTime / movementDuration;

            mainCamera.transform.position = new Vector3(
                Mathf.SmoothStep(originalPosition.x, targetTransform.position.x, t),
                Mathf.SmoothStep(originalPosition.y, targetTransform.position.y, t),
                Mathf.SmoothStep(originalPosition.z, targetTransform.position.z, t)
            );
            mainCamera.transform.localEulerAngles = new Vector3(
                Mathf.SmoothStep(originalLocalEulerAngles.x, targetTransform.localEulerAngles.x, t),
                Mathf.SmoothStep(originalLocalEulerAngles.y, targetTransform.localEulerAngles.y, t),
                Mathf.SmoothStep(originalLocalEulerAngles.z, targetTransform.localEulerAngles.z, t)
            );
        }
    }

    public void ChangePosition (CameraPosition position, float duration = 1) {
        var camera = defaultCameras[(int)position];

        movementTime = 0;
        movementDuration = duration;
        originalPosition = mainCamera.transform.position;
        originalLocalEulerAngles = mainCamera.transform.localEulerAngles;
        targetTransform = camera.transform;
    }
}
