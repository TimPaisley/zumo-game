using UnityEngine;

namespace Zumo {
	abstract class AnimalAbility {
		public bool canBePerformed { get; protected set; }

		public bool disableControl { get; protected set; }

		public int massIncrease { get; protected set; }

		public abstract void Perform ();
	}
}
