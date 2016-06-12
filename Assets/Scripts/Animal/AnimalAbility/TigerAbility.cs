using UnityEngine;
using System.Collections;

public class TigerAbility : MonoBehaviour,AnimalAbility {

	private AnimalController animal;
	private bool isAvailable = true;
	private bool isActive = false;
	public float ticker = 10.0f;
	private SkinnedMeshRenderer rend;
	public Color[] colors;
    public bool flash = false;

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
				for (int i = 0; i < rend.materials.Length; i++) {
					rend.materials [i].color = colors [i];
				}
                animal.tigerAbility = false;
                print ("Disabled Tiger ability");
			} else {
				Color newColour = getNextColor(rend.material.GetColor("_Color"));
				foreach(Material m in rend.materials){
					m.color = newColour;
				}
			}
		} else {
			return;
		}
	}

	private Color getNextColor(Color currentColor)
	{
        return Color.black;
    }


	public void applyAbility (){
		if (isAvailable) {
			print ("Tiger Ability");
			Color newColour = getNextColor(rend.material.GetColor("_Color"));
			foreach(Material m in rend.materials){
				m.color = newColour;
			}
            animal.tigerAbility = true;
          
            isActive = true;
		}
		isAvailable = false;
	}
}
