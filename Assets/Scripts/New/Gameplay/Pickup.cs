using UnityEngine;

namespace Zumo {
	public enum PickupType {
		Speed,
		Mass,
		TimeStop,
		Bomb
	}

	public class Pickup : MonoBehaviour {
		public PickupType type;
        public AudioClip pickupSound;

		[Header("Speed/Mass/TimeStop")]
		public float duration = 0f;
		public float speedIncrease = 0f;
		public float massIncrease = 0f;

		[Header("Bomb")]
		public float fuseTime = 0f;
		public float power = 0f;

        public Animal owner { get; set; }
	}
}
