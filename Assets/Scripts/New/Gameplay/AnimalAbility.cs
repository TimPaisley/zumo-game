using UnityEngine;

namespace Zumo {
	abstract class AnimalAbility : MonoBehaviour {
		public bool canBePerformed { get; protected set; }

		public bool disableControl { get; protected set; }

		public bool disableRecoil { get; protected set; }

		public float massIncrease { get; protected set; }

		public abstract void Perform ();

		void Awake () {
			canBePerformed = true;
		}
	}
}
