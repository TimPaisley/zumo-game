using UnityEngine;
using System.Linq;

namespace Zumo {
	public class Board : MonoBehaviour {
        public AudioClip music;
        public AudioClip intro;

		public Transform[] animalSpawnPoints { get; private set; }
        public Transform[] pickupSpawnPoints { get; private set; }

		void Awake () {
			animalSpawnPoints = GetComponentsInChildren<AnimalSpawnPoint>().Select(spawn => spawn.transform).ToArray();
            pickupSpawnPoints = GetComponentsInChildren<PickupSpawnPoint>().Select(spawn => spawn.transform).ToArray();
        }
	}
}
