using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

namespace Zumo {
	class BoardChoiceSpinner : MonoBehaviour {
		public RectTransform baseSpinner;

		RectTransform[] spinners;
		List<float> pastFewAngles = new List<float>(new float[] { 0, 0, 0, 0, 0 });

		void Awake () {
			baseSpinner.gameObject.SetActive(false);
		}

		public IEnumerator SpinToBoard (ChoosableBoard chosenBoard) {
			var targetAngle = 720f + chosenBoard.indicatorAngle;

			spinners = new [] {
				createSpinnerShadow(1f),
				createSpinnerShadow(0.8f),
				createSpinnerShadow(0.6f),
				createSpinnerShadow(0.4f),
				createSpinnerShadow(0.2f)
			};

			return spinSpinner(targetAngle);
		}

		IEnumerator spinSpinner (float targetAngle) {
			var startTime = Time.fixedTime;

			while (pastFewAngles.Last() < targetAngle) {
				pastFewAngles.Insert(0, expoEase(0, targetAngle, Time.fixedTime - startTime));
				pastFewAngles.RemoveAt(pastFewAngles.Count - 1);

				for (var i = 0; i < spinners.Length; i++) {
					spinners[i].localEulerAngles = new Vector3(0, 0, pastFewAngles[i] % 360);
				}

				yield return new WaitForEndOfFrame();
			}

			yield return new WaitForSeconds(1f);

			cleanup();
		}

		void cleanup() {
			foreach (var spinner in spinners) {
				Destroy(spinner.gameObject);
			}
		}

		RectTransform createSpinnerShadow (float opacity) {
			var shadow = Instantiate(baseSpinner);
			shadow.gameObject.SetActive(true);
			shadow.transform.SetParent(baseSpinner.parent, false);
			shadow.anchoredPosition = Vector2.zero;

			var image = shadow.GetComponent<Image>();
			image.color = new Color(image.color.r, image.color.g, image.color.b, opacity);

			return shadow;
		}

		float expoEase (float from, float to, float t) {
			var change = to - from;

			return (t >= 1) ?
				from + change :
				change * (-Mathf.Pow(2, -10 * t) + 1) + from;
		}
	}
}
