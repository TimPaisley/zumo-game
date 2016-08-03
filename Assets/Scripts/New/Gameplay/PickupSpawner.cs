using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Zumo {
	public class PickupSpawner : MonoBehaviour {
        const int MAX_SPAWNED_PICKUPS = 2;

        public float delay;
        
        Pickup[] possiblePickups;
        Transform[] spawnPoints;

        List<Pickup> spawnedPickups = new List<Pickup>();
        Coroutine spawner;

        void Awake () {
            possiblePickups = GetComponentsInChildren<Pickup>();

            foreach (var pickup in possiblePickups) {
                pickup.gameObject.SetActive(false);
            }
        }

        public void Enable (Transform[] spawnPoints) {
            this.spawnPoints = spawnPoints;

            spawner = StartCoroutine(waitUntilNextSpawnAllowed());
        }

        public void Disable () {
            if (spawner != null) {
                StopCoroutine(spawner);
            }

            foreach (var pickup in spawnedPickups) {
                Destroy(pickup);
            }
        }

        IEnumerator waitUntilNextSpawnAllowed () {
            while (spawnedPickups.Count >= MAX_SPAWNED_PICKUPS) {
                yield return new WaitForEndOfFrame();
                updateSpawnedPickups();
            }
            
            yield return new WaitForSeconds(delay);
            updateSpawnedPickups();

            spawnPickup();

            spawner = StartCoroutine(waitUntilNextSpawnAllowed());
        }

        void spawnPickup () {
            Debug.Log("Spawning pickup!");
            var allowedPickups = possiblePickups.Where(pickup => !spawnedPickups.Contains(pickup));

            var spawnedPickup = Instantiate(allowedPickups.Sample());

            var illegalPositions = spawnedPickups.Select(pickup => pickup.transform.position);
            var allowedSpawnPoints = spawnPoints.Where(point => !illegalPositions.Contains(point.position));

            spawnedPickup.transform.position = allowedSpawnPoints.Sample().position;
            spawnedPickup.gameObject.SetActive(true);

            spawnedPickups.Add(spawnedPickup);
        }

        void updateSpawnedPickups () {
            spawnedPickups = spawnedPickups.Where(pickup => pickup.owner == null).ToList();
        }
    }
}
