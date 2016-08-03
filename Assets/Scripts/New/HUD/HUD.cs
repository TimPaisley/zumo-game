using System.Collections.Generic;
using UnityEngine;

namespace Zumo {
	public class HUD : MonoBehaviour {
        AnimalIndicator basePlayerIndicator;

        Dictionary<Animal, AnimalIndicator> animalIndicators = new Dictionary<Animal, AnimalIndicator>();

        void Awake () {
            basePlayerIndicator = GetComponentInChildren<AnimalIndicator>();
            basePlayerIndicator.gameObject.SetActive(false);
        }

        void Update () {
            foreach (var pair in animalIndicators) {
                pair.Value.gameObject.SetActive(pair.Key.isAlive);
            }
        }

        public void Setup (IEnumerable<Animal> playerAnimals) {
            foreach (var animal in playerAnimals) {
                animalIndicators[animal] = createAnimalIndicator(animal);
            }
        }

        AnimalIndicator createAnimalIndicator(Animal animal) {
            var indicator = Instantiate(basePlayerIndicator);
            indicator.transform.SetParent(basePlayerIndicator.transform.parent, false);
            indicator.Setup(animal);

            return indicator;
        }
    }
}
