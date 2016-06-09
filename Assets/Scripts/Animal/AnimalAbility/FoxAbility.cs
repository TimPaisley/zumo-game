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
		float velocity = 0.05f;
		float max = 1.0f;
		float min = 0.0f;
		float r = currentColor.r;
		float g = currentColor.g;
		float b = currentColor.b;
		if (r >= max)
		{
			if (b > min) { return new Color(max, min, b - velocity); } // decreasing previous
			else if (g < max) { return new Color(max, g + velocity, min); } //increase next
			else { return new Color(max - velocity, max, min); } // next one increased, decrease this one once and hand over to g==1
		}
		else if (g >= max)
		{
			if (r > min) { return new Color(r - velocity, max, min); }
			else if (b < max) { return new Color(min, g, b + velocity); }
			else { return new Color(min, max - velocity, max); }

		}
		else if (b >= 1)
		{
			if (g > min) { return new Color(min, g - velocity, max); }
			else if (r <max) { return new Color(r + velocity, min, max); }
			else { return new Color(max, min, max - velocity); }
		}
		return new Color(max, min, min);
	}


	public void applyAbility (){
		if (isAvailable) {
			print ("Fox Ability");
			Color newColour = getNextColor(rend.material.GetColor("_Color"));
			foreach(Material m in rend.materials){
				m.color = newColour;
			}
			this.transform.tag = "Untagged";
			isActive = true;
		}
		isAvailable = false;
	}
}
