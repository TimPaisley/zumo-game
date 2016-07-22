using System.Collections;
using UnityEngine;

namespace Zumo {
    public class CameraManager : MonoBehaviour {
        public Camera defaultCamera;

        Camera currentCamera;

        Coroutine currentMovement;
        Coroutine currentShake;

        void Start () {
            defaultCamera.gameObject.SetActive(false);
            
            setCurrentCamera(defaultCamera);
        }

        public void Use (Camera camera, float animationDuration = 1) {
            if (currentMovement != null) {
                StopCoroutine(currentMovement);
            }

            currentMovement = StartCoroutine(transitionToCamera(camera, animationDuration));
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

        private IEnumerator transitionToCamera(Camera target, float duration) {
            var originalPosition = currentCamera.transform.position;
            var originalLocalEulerAngles = currentCamera.transform.localEulerAngles;
            var targetPosition = target.transform.position;
            var targetLocalEulerAngles = smallestAngleDifference(target.transform.localEulerAngles, originalLocalEulerAngles);
            
            var elapsed = 0f;

            while (elapsed < duration) {
                var t = elapsed / duration;

                currentCamera.transform.position = new Vector3(
                    Mathf.SmoothStep(originalPosition.x, targetPosition.x, t),
                    Mathf.SmoothStep(originalPosition.y, targetPosition.y, t),
                    Mathf.SmoothStep(originalPosition.z, targetPosition.z, t)
                );
                currentCamera.transform.localEulerAngles = new Vector3(
                    Mathf.SmoothStep(originalLocalEulerAngles.x, targetLocalEulerAngles.x, t),
                    Mathf.SmoothStep(originalLocalEulerAngles.y, targetLocalEulerAngles.y, t),
                    Mathf.SmoothStep(originalLocalEulerAngles.z, targetLocalEulerAngles.z, t)
                );

                elapsed += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            setCurrentCamera(target);
        }

        IEnumerator shakeCamera(float intensity, float period) {
            var originalPosition = currentCamera.transform.position;

            while (true) {
                currentCamera.transform.position = new Vector3(
                    originalPosition.x + Mathf.Sin(Time.fixedTime / period) * intensity,
                    originalPosition.y + Mathf.Sin(Time.fixedTime / period + Mathf.PI) * intensity,
                    originalPosition.z + Mathf.Sin(Time.fixedTime / period + Mathf.PI / 2) * intensity
                );

                yield return new WaitForEndOfFrame();
            }
        }

        void setCurrentCamera(Camera camera) {
            if (currentCamera) Destroy(currentCamera.gameObject);

            currentCamera = Instantiate(camera);
            currentCamera.gameObject.SetActive(true);
        }

        Vector3 smallestAngleDifference(Vector3 targetAngle, Vector3 originAngle) {
            if (targetAngle.x - originAngle.x < -180) {
                targetAngle.x += 360;
            } else if (targetAngle.x - originAngle.x > 180) {
                targetAngle.x -= 360;
            }

            if (targetAngle.y - originAngle.y < -180) {
                targetAngle.y += 360;
            } else if (targetAngle.y - originAngle.y > 180) {
                targetAngle.y -= 360;
            }

            if (targetAngle.z - originAngle.z < -180) {
                targetAngle.z += 360;
            } else if (targetAngle.z - originAngle.z > 180) {
                targetAngle.z -= 360;
            }

            return targetAngle;
        }
    }
}
