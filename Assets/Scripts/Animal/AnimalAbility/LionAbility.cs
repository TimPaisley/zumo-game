using UnityEngine;
using System.Collections;

public class LionAbility : MonoBehaviour,AnimalAbility {

	private AnimalController animal;
	private bool isAvailable = true;
	private float roarForce = 50.0f;
    private bool isActive = false;
    public float ticker = 1.0f;
    public ParticleSystem lionPS;
    private ParticleSystem.EmissionModule lionEM;

    void Start(){
		animal = GetComponent<AnimalController>();
	}
		
    void Update () {
		if (isActive) {
			ticker -= Time.deltaTime;
			if (ticker < 0.0f) {
				isActive = false;
				lionEM.enabled = false;
				lionPS.Stop ();
				lionPS.Clear ();
			}
		} else {
			return;
		}
	}

	public void applyAbility (){
		if (isAvailable) {
			print ("Lion Ability");
			animal.lionAbility = true;
			roar(this.transform.position,roarForce);
            isActive = true;
        }
		isAvailable = false;
	}

	public void roar (Vector3 pos, float pow) {
		AnimalController[] animals = FindObjectsOfType<AnimalController> ();
        lionPS.Simulate(0.0f, true, true);
        lionEM.enabled = true;
        lionPS.Play();
        foreach (AnimalController a in animals) {
			if (!a.lionAbility && !a.foxAbility) {
				Vector3 awayFromBomb = (a.transform.position - pos);
				a.rb.AddForce ((awayFromBomb.normalized  + new Vector3(0,1,0)) * (pow / awayFromBomb.magnitude*1.5f), ForceMode.Impulse);
				Debug.Log ((awayFromBomb.normalized  + new Vector3(0,1,0)) * (1 / awayFromBomb.magnitude));
			}
		}
       
        animal.lionAbility = false;
	}

}
