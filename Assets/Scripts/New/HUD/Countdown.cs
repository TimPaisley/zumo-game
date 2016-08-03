using UnityEngine;
using System.Collections;
using System.Linq;

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
            foreach (var step in steps.Take(steps.Length - 1)) {
                var currentStep = step;
                currentStep.gameObject.SetActive(true);

                numberSound.Play();
                yield return new WaitForSeconds(1f);

                currentStep.gameObject.SetActive(false);
            }

            steps.Last().gameObject.SetActive(true);
		}

        public void Stop () {
            foreach (var step in steps) {
                step.gameObject.SetActive(false);
            }
        }
	}
}
