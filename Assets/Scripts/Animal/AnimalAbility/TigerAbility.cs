using UnityEngine;
using System.Collections;

public class TigerAbility : MonoBehaviour,AnimalAbility {

    //private Rigidbody rb;
	private AnimalController animal;
	private bool isAvailable = true;
	private bool isActive = false;
	public float ticker = 10.0f;
	private SkinnedMeshRenderer rend;
	public Color[] colors;
    public GameObject head;
	//SkinnedMeshRenderer smr;
	void Start(){
		//rb = this.GetComponent<Rigidbody>();
		animal = GetComponent<AnimalController>();
		rend = GetComponentInChildren<SkinnedMeshRenderer>();  
		colors = new Color[rend.materials.Length];	
		for (int i = 0; i < rend.materials.Length; i++) {
			colors [i] = rend.materials [i].color;
		}
	}

	void Update () {
		if (isActive) {
			ticker -= Time.deltaTime;
			if (ticker < 0.0f) {
				isActive = false;

                print ("Disabled Tiger ability");
			} 
		} else {
			return;
		}
	}



	public void applyAbility (){
		if (isAvailable) {
            print("Tiger Ability");

            
             isActive = true;
		}
		isAvailable = false;
	}
}
