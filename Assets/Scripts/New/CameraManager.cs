using System;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;
using Zumo.InputHelper;

namespace Zumo {
    class CameraManager : MonoBehaviour {
        [Header("Main camera")]
        public Camera mainCamera;

        [Header("Camera positions")]
        public Camera splashCamera;
        public Camera menuCamera;
        public Camera tiltedMenuCamera;
        public Camera gameCamera;
        public Camera blackoutCamera;

        private Coroutine currentMovement;
        private Coroutine currentShake;

        void Start () {
            mainCamera.gameObject.SetActive(true);

            splashCamera.gameObject.SetActive(false);
            menuCamera.gameObject.SetActive(false);
            tiltedMenuCamera.gameObject.SetActive(false);
            gameCamera.gameObject.SetActive(false);
            blackoutCamera.gameObject.SetActive(false);
        }

        public void Use (Camera camera, float animationDuration = 1) {
            if (currentMovement != null) {
                StopCoroutine(currentMovement);
            }

            currentMovement = StartCoroutine(moveCamera(camera.transform, animationDuration));
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

        private IEnumerator moveCamera(Transform target, float duration) {
            var originalPosition = mainCamera.transform.position;
            var originalLocalEulerAngles = mainCamera.transform.localEulerAngles;

            var elapsed = 0f;

            while (elapsed < duration) {
                var t = elapsed / duration;

                mainCamera.transform.position = new Vector3(
                    Mathf.SmoothStep(originalPosition.x, target.position.x, t),
                    Mathf.SmoothStep(originalPosition.y, target.position.y, t),
                    Mathf.SmoothStep(originalPosition.z, target.position.z, t)
                );
                mainCamera.transform.localEulerAngles = new Vector3(
                    Mathf.SmoothStep(originalLocalEulerAngles.x, target.localEulerAngles.x, t),
                    Mathf.SmoothStep(originalLocalEulerAngles.y, target.localEulerAngles.y, t),
                    Mathf.SmoothStep(originalLocalEulerAngles.z, target.localEulerAngles.z, t)
                );

                elapsed += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator shakeCamera(float intensity, float period) {
            var originalPosition = mainCamera.transform.position;

            while (true) {
                mainCamera.transform.position = new Vector3(
                    originalPosition.x + Mathf.Sin(Time.fixedTime / period) * intensity,
                    originalPosition.y + Mathf.Sin(Time.fixedTime / period + Mathf.PI) * intensity,
                    originalPosition.z + Mathf.Sin(Time.fixedTime / period + Mathf.PI / 2) * intensity
                );

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
