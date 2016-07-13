using UnityEngine;
using System.Linq;

namespace Zumo {
	public class Board : MonoBehaviour {
		public Transform[] spawnPoints { get; private set; }

		void Awake () {
			spawnPoints = GetComponentsInChildren<SpawnPoint>().Select(spawn => spawn.transform).ToArray();
		}
	}
}
