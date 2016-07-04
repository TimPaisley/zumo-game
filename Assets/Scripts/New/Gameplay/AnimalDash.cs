using System.Collections;
using UnityEngine;

namespace Zumo {
	class AnimalDash : MonoBehaviour {
		const float CHARGE_LEVELS = 3f;

		// Configuration

		[Header("Timing")]
		public float chargeDuration = 3f;
		public float dashDuration = 0.5f;
		public float cooldownDuration = 4f;

		[Header("Power")]
		public float minSpeedIncrease;
		public float maxSpeedIncrease;
		public float minMassIncrease;
		public float maxMassIncrease;

		// Internal State

		float chargedSpeedIncrease;
		float chargedMassIncrease;

		Coroutine dashCharge;
		Coroutine dash;
		Coroutine dashCooldown;

		// Public State

		public float speedIncrease { get; private set; }

		public float massIncrease { get; private set; }

		public bool isCharging { get { return dashCharge != null; } }

		public bool isDashing { get { return dash != null; } }

		public bool isCoolingDown { get { return dashCooldown != null; } }

		public bool isInactive { get { return !isCharging && !isDashing && !isCoolingDown; } }

		// Actions

		public void Stop () {
			stopCoroutines();

			speedIncrease = 0;
			massIncrease = 0;
		}

		public void StartDashCharging () {
			stopCoroutines();
			dashCharge = StartCoroutine(chargeDash());
		}

		public void StartDashing () {
			stopCoroutines();
			dash = StartCoroutine(performDash());
		}

		IEnumerator chargeDash () {
			chargedSpeedIncrease = minSpeedIncrease;
			chargedMassIncrease = minMassIncrease;

			for (var i = 0; i < CHARGE_LEVELS; i++) {
				yield return new WaitForSeconds(chargeDuration / CHARGE_LEVELS);

				chargedMassIncrease += (maxMassIncrease - minMassIncrease) / CHARGE_LEVELS;
				chargedSpeedIncrease += (maxSpeedIncrease - minSpeedIncrease) / CHARGE_LEVELS;
			}
		}

		IEnumerator performDash () {
			speedIncrease = chargedSpeedIncrease;
			massIncrease = chargedMassIncrease;

			yield return new WaitForSeconds(dashDuration);

			speedIncrease = 0;
			massIncrease = 0;

			stopCoroutines();
			dashCooldown = StartCoroutine(cooldown());
		}

		IEnumerator cooldown () {
			yield return new WaitForSeconds(cooldownDuration);

			Stop();
		}

		void stopCoroutines () {
			if (dashCharge != null) {
				StopCoroutine(dashCharge);
				dashCharge = null;
			}

			if (dash != null) {
				StopCoroutine(dash);
				dash = null;
			}

			if (dashCooldown != null) {
				StopCoroutine(dashCooldown);
				dashCooldown = null;
			}
		}
	}
}
