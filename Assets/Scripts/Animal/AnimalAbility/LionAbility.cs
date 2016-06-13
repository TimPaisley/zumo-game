using UnityEngine;
using System.Collections;

public class LionAbility : MonoBehaviour,AnimalAbility {

	private AnimalController animal;
	private bool isAvailable = true;

	void Start(){
		animal = GetComponent<AnimalController>();
	}

	void Update () {
		
	}

	public void applyAbility (){
		if (isAvailable) {
			print ("Lion Ability");

		}
		isAvailable = false;
	}
}
