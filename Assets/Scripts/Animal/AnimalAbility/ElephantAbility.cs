using UnityEngine;
using System.Collections;

public class ElephantAbility : MonoBehaviour,AnimalAbility {

	private bool isAvailable = true;
	private bool isActive = false;
	public float ticker = 5.0f;

	public bool shake = false;
	public Camera mainCam;
	private float originalPostion;
	private Renderer rend;
	public Color[] colors;

	void Start(){
		originalPostion = mainCam.transform.localPosition.x;
		Renderer[] r = GetComponentsInChildren<Renderer> ();
		rend = r [1];
		colors = new Color[rend.materials.Length];	
		for (int i = 0; i < rend.materials.Length; i++) {
			colors [i] = rend.materials [i].color;
		}
	}

	void Update () {
		Vector3 camPostion;
		if (isActive) {
			ticker -= Time.deltaTime;
			camPostion = mainCam.transform.localPosition;
			float x = (originalPostion + Mathf.Sin(ticker * 8f));
			camPostion = new Vector3 (x, camPostion.y, camPostion.z);
			mainCam.transform.localPosition = camPostion;

			if (ticker < 0.0f) {
				isActive = false;
				mainCam.gameObject.SetActive(false);
				camPostion = mainCam.transform.localPosition;
				camPostion.x = originalPostion;
				mainCam.transform.localPosition = camPostion;
				for (int i = 0; i < rend.materials.Length; i++) {
					rend.materials [i].color = colors [i];
				}
				print ("Disabled Elephant ability");
			} 
		}
	}

	public void applyAbility (){
		if (isAvailable) {
			print ("Elephant Ability");
			mainCam.gameObject.SetActive(true);
			isActive = true;
			foreach(Material m in rend.materials){
				m.color = Color.red;
			}

		}
		isAvailable = false;
	}
}
