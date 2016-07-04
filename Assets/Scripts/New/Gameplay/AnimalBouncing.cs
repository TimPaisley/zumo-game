using UnityEngine;

namespace Zumo {
	class AnimalBouncing : MonoBehaviour {
		public bool isGrounded { get; private set; }
		public bool knockedBack { get; private set; }

		public void BounceAwayFrom (Animal other) {
		}

		public void RecoilFrom (Animal other) {
		}
	}
}
