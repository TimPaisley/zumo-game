using UnityEngine;
using System.Collections;

public class FoxAbility : MonoBehaviour,AnimalAbility {
	//private Rigidbody rb;
	private AnimalController animal;
	private bool isAvailable = true;
	private bool isActive = false;
	public float ticker = 10.0f;
	private Renderer rend;
	public Color[] colors;
	public float transparency = 0.5f;

	void Start(){
		//rb = this.GetComponent<Rigidbody>();
		animal = GetComponent<AnimalController>();
		Renderer[] r = GetComponentsInChildren<Renderer> ();
		rend = r [1];
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

				print ("Disabled Fox ability");

			}
		} else {
			return;
		}
	}

	public void applyAbility (){
		if (isAvailable) {
			print ("Fox Ability");

			foreach(Material m in rend.materials){
				Color color = new Color(m.color.a/2, m.color.r, m.color.g, m.color.b);
				//color.a = 0.1f;
				m.color = color;
				//m.color = Color.clear;

			}
			isActive = true;
		}
		isAvailable = false;
	}
}
