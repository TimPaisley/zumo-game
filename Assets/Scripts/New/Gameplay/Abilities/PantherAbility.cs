using System.Collections;
using UnityEngine;

namespace Zumo {
	public class PantherAbility : AnimalAbility {
		public float duration = 5f;

		public override void Perform () {
			StartCoroutine(stopAfterDuration());
		}

		IEnumerator stopAfterDuration () {
			yield return new WaitForSeconds(duration);
		}
	}
}
