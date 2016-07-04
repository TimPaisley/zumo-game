using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Zumo {
	class AnimalPickups : MonoBehaviour {
		static Player timeStopper;

		Animal animal;
		List<Pickup> activePickups = new List<Pickup>();

		public float speedIncrease { get; private set; }
		public float massIncrease { get; private set; }

		public bool disableControl { get { return timeStopper != null && timeStopper != animal.player; } }

		void Awake () {
			animal = GetComponent<Animal>();
		}

		public void PickUp (Pickup pickup) {
			apply(pickup);

			if (pickup.duration > 0) {
				pickup.owner = animal.player;
				activePickups.Add(pickup);
				StartCoroutine(expirePickup(pickup));
			} else {
				Destroy(pickup.gameObject);
			}
		}

		IEnumerator expirePickup (Pickup pickup) {
			yield return new WaitForSeconds(pickup.duration);

			revert(pickup);

			activePickups.Remove(pickup);
			Destroy(pickup.gameObject);
		}

		void apply (Pickup pickup) {
			switch (pickup.type) {
			case PickupType.Speed:
				speedIncrease += pickup.speedIncrease;
				break;
			case PickupType.Mass:
				massIncrease += pickup.massIncrease;
				break;
			case PickupType.TimeStop:
				timeStopper = animal.player;
				break;
			case PickupType.Bomb:
				StartCoroutine(deployBomb(pickup));
				break;
			}
		}

		void revert (Pickup pickup) {
			switch (pickup.type) {
			case PickupType.Speed:
				speedIncrease -= pickup.speedIncrease;
				break;
			case PickupType.Mass:
				massIncrease -= pickup.massIncrease;
				break;
			case PickupType.TimeStop:
				if (timeStopper == animal.player) {
					timeStopper = null;
				}
				break;
			case PickupType.Bomb:
				// Everything is handled by deployBomb()
				break;
			}
		}

		IEnumerator deployBomb (Pickup bomb) {
			// ... spawn bomb

			yield return new WaitForSeconds(bomb.fuseTime);

			// ... detonate
		}
	}
}
