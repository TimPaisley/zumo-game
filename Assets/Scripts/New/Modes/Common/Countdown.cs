using UnityEngine;
using System.Collections;

namespace Zumo {
	public class Countdown : MonoBehaviour {
        CountdownStep[] steps;
        AudioSource numberSound;

        void Awake () {
            steps = GetComponentsInChildren<CountdownStep>();
            numberSound = GetComponent<AudioSource>();

            foreach (var step in steps) {
                step.gameObject.SetActive(false);
            }
        }

		public IEnumerator Play () {
            CountdownStep currentStep = null;

            foreach (var step in steps) {
                if (currentStep != null) {
                    currentStep.gameObject.SetActive(false);
                }

                currentStep = step;
                currentStep.gameObject.SetActive(true);

                numberSound.Play();
                yield return new WaitForSeconds(1f);
            }

            currentStep.gameObject.SetActive(false);
		}
	}
}
