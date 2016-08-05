using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace Zumo {
	public class AnimalPickups : MonoBehaviour {
		static Player timeStopper;

        public ParticleSystem speedParticles;
        public float massScale;

        Animal animal;
        InterestingAudioSource audio;
		List<Pickup> activePickups = new List<Pickup>();

		public float speedIncrease { get; private set; }
		public float massIncrease { get; private set; }

		public bool disableControl { get { return timeStopper != null && timeStopper != animal.player; } }

		void Awake () {
			animal = GetComponent<Animal>();
            audio = new InterestingAudioSource(gameObject);

            speedParticles.Stop();
            speedParticles.Clear();
        }

		public void PickUp (Pickup pickup) {
			apply(pickup);

            if (pickup.pickupSound) {
                audio.PlayOnce(pickup.pickupSound);
            }

			if (pickup.duration > 0) {
                pickup.owner = animal; // Make sure the pickup manager detects it's been collected
                pickup.gameObject.SetActive(false);

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
                speedParticles.Play();
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
                speedParticles.Stop();
                speedParticles.Clear();
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
