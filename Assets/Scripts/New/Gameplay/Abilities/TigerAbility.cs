using System.Collections;
using UnityEngine;

namespace Zumo {
	public class TigerAbility : AnimalAbility {
		public float duration = 5f;

		public override void Perform () {
			canBePerformed = false;

			disableRecoil = true;

			StartCoroutine(stopAfterDuration());
		}

		IEnumerator stopAfterDuration () {
			yield return new WaitForSeconds(duration);

			disableRecoil = false;
		}
	}
}
