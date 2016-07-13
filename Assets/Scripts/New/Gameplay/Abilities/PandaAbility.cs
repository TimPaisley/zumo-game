using System.Collections;
using UnityEngine;

namespace Zumo {
	public class PandaAbility : AnimalAbility {
		public float duration = 5f;

		public override void Perform () {
			canBePerformed = false;

			disableControl = true;
			disableRecoil = true;
			massIncrease = 10000f;

			StartCoroutine(stopAfterDuration());
		}

		IEnumerator stopAfterDuration () {
			yield return new WaitForSeconds(duration);

			disableControl = false;
			disableRecoil = false;
			massIncrease = 0;
		}
	}
}
