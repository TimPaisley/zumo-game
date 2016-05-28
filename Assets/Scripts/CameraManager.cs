using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {
    public Camera menuCamera;
    public Camera tiltedMenuCamera;
    public Camera gameCamera;

    public Camera mainCamera { get; private set; }

    private float movementTime;
    private float movementDuration;
    private Vector3 originalPosition;
    private Vector3 originalLocalEulerAngles;
    private Transform targetTransform;

	void Start () {
        mainCamera = Instantiate(menuCamera);

        menuCamera.gameObject.SetActive(false);
        gameCamera.gameObject.SetActive(false);
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

    public void Use (Camera camera, float animationDuration = 1) {
        movementTime = 0;
        movementDuration = animationDuration;

        originalPosition = mainCamera.transform.position;
        originalLocalEulerAngles = mainCamera.transform.localEulerAngles;
        targetTransform = camera.transform;
    }
}
