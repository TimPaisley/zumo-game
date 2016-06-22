using System.Collections;
using UnityEngine;

namespace Zumo {
    class CameraManager : MonoBehaviour {
        [Header("Main Camera")]
        public Camera defaultCamera;

        private Camera currentCamera;

        private Coroutine currentMovement;
        private Coroutine currentShake;

        void Start () {
            defaultCamera.gameObject.SetActive(true);

            currentCamera = defaultCamera;
        }

        public void Use (Camera camera, float animationDuration = 1) {
            if (currentMovement != null) {
                StopCoroutine(currentMovement);
            }

            currentMovement = StartCoroutine(moveCamera(camera, animationDuration));
        }

        public void Shake (float intensity, float period = 1) {
            if (currentShake != null) {
                StopCoroutine(currentShake);
            }

            currentShake = StartCoroutine(shakeCamera(intensity, period));
        }

        public void StopShaking () {
            if (currentShake != null) {
                StopCoroutine(currentShake);
            }
        }

        private IEnumerator moveCamera(Camera target, float duration) {
            var targetPosition = target.transform.position;
            var targetLocalEulerAngles = target.transform.localEulerAngles;
            var originalPosition = defaultCamera.transform.position;
            var originalLocalEulerAngles = defaultCamera.transform.localEulerAngles;

            var elapsed = 0f;

            while (elapsed < duration) {
                var t = elapsed / duration;

                defaultCamera.transform.position = new Vector3(
                    Mathf.SmoothStep(originalPosition.x, targetPosition.x, t),
                    Mathf.SmoothStep(originalPosition.y, targetPosition.y, t),
                    Mathf.SmoothStep(originalPosition.z, targetPosition.z, t)
                );
                defaultCamera.transform.localEulerAngles = new Vector3(
                    Mathf.SmoothStep(originalLocalEulerAngles.x, targetLocalEulerAngles.x, t),
                    Mathf.SmoothStep(originalLocalEulerAngles.y, targetLocalEulerAngles.y, t),
                    Mathf.SmoothStep(originalLocalEulerAngles.z, targetLocalEulerAngles.z, t)
                );

                elapsed += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            setCurrentCamera(target);
        }

        private IEnumerator shakeCamera(float intensity, float period) {
            var originalPosition = defaultCamera.transform.position;

            while (true) {
                defaultCamera.transform.position = new Vector3(
                    originalPosition.x + Mathf.Sin(Time.fixedTime / period) * intensity,
                    originalPosition.y + Mathf.Sin(Time.fixedTime / period + Mathf.PI) * intensity,
                    originalPosition.z + Mathf.Sin(Time.fixedTime / period + Mathf.PI / 2) * intensity
                );

                yield return new WaitForEndOfFrame();
            }
        }

        private void setCurrentCamera(Camera camera) {
            currentCamera.enabled = false;
            currentCamera = camera;
            currentCamera.enabled = true;
        }
    }
}
