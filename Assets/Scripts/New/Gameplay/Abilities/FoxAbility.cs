using System.Collections;
using UnityEngine;

namespace Zumo {
	class FoxAbility : AnimalAbility {
		public float duration = 5f;

		bool disableCollisions;

		public override void Perform () {
			canBePerformed = false;

			disableRecoil = true;
			disableCollisions = true;

			StartCoroutine(stopAfterDuration());
		}

		IEnumerator stopAfterDuration () {
			yield return new WaitForSeconds(duration);

			disableRecoil = false;
			disableCollisions = false;
		}
	}
}
