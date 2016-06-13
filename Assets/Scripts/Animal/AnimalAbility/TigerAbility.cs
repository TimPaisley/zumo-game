using UnityEngine;
using System.Collections;

public class TigerAbility : MonoBehaviour,AnimalAbility {

	private AnimalController animal;
	private bool isAvailable = true;
	private bool isActive = false;
	public float ticker = 10.0f;
	public ParticleSystem tigerPS;
	private ParticleSystem.EmissionModule tigerEM;

	void Start(){
		animal = GetComponent<AnimalController>();
		tigerEM = tigerPS.emission;
	}

	void Update () {
		if (isActive) {
			ticker -= Time.deltaTime;
			if (ticker < 0.0f) {
				isActive = false;
				tigerEM.enabled = false;
				tigerPS.Stop ();
				tigerPS.Clear ();
                animal.tigerAbility = false;
                print ("Disabled Tiger ability");
			}
		} else {
			return;
		}
	}
		
	public void applyAbility (){
		if (isAvailable) {
			print ("Tiger Ability");
			tigerPS.Simulate(0.0f,true,true);
			tigerEM.enabled = true;
			tigerPS.Play ();
            animal.tigerAbility = true;
            isActive = true;
		}
		isAvailable = false;
	}
}
