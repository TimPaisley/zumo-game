using UnityEngine;
using System.Collections;

public class LionAbility : MonoBehaviour,AnimalAbility {

	private AnimalController animal;
	private bool isAvailable = true;
	private float roarForce = 50.0f;
	void Start(){
		animal = GetComponent<AnimalController>();
	}
		
	public void applyAbility (){
		if (isAvailable) {
			print ("Lion Ability");
			animal.lionAbility = true;
			roar(this.transform.position,roarForce);
		}
		isAvailable = false;
	}

	public void roar (Vector3 pos, float pow) {
		AnimalController[] animals = FindObjectsOfType<AnimalController> ();
		foreach (AnimalController a in animals) {
			if (!a.lionAbility) {
				Vector3 awayFromBomb = (a.transform.position - pos);
				a.rb.AddForce ((awayFromBomb.normalized  + new Vector3(0,1,0)) * (pow / awayFromBomb.magnitude*1.5f), ForceMode.Impulse);
				Debug.Log ((awayFromBomb.normalized  + new Vector3(0,1,0)) * (1 / awayFromBomb.magnitude));
			}
		}
		animal.lionAbility = false;
	}

}
