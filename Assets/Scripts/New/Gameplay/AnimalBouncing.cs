using UnityEngine;
using System.Collections;

namespace Zumo {
	class AnimalBouncing : MonoBehaviour {
		const float KNOCKBACK_TIMEOUT = 0.2f;

		public bool isGrounded { get; private set; }
		public bool knockedBack { get; private set; }

		GameManager gm;
		Rigidbody rigidBody;
		Coroutine knockbackTimer;

		void Awake () {
			gm = FindObjectOfType<GameManager>();
			rigidBody = GetComponent<Rigidbody>();
		}

		public void BounceAwayFrom (Animal other) {
			bounceOff(other, other.rigidBody.velocity.normalized);
		}

		public void RecoilFrom (Animal other) {
			bounceOff(other, -transform.forward);
		}

		void bounceOff (Animal other, Vector3 direction) {
			var bounceDir = new Vector3(direction.x, 0.0f, direction.z).normalized + Vector3.up;

			var bounceForce = bounceDir * other.rigidBody.velocity.magnitude * other.rigidBody.mass * gm.bounceForce;

			rigidBody.velocity = Vector3.zero;
			rigidBody.AddForce(bounceForce, ForceMode.Impulse);

			knockedBack = true;
			startKnockbackTimer();
		}

		void startKnockbackTimer () {
			if (knockbackTimer != null) {
				StopCoroutine(knockbackTimer);
			}

			knockbackTimer = StartCoroutine(timeoutKnockback());
		}

		IEnumerator timeoutKnockback () {
			yield return new WaitForSeconds(KNOCKBACK_TIMEOUT);

			knockedBack = false;
		}
	}
}
