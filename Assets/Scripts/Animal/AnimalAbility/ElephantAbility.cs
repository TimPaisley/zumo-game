using UnityEngine;
using System.Collections;

public class ElephantAbility : MonoBehaviour,AnimalAbility {

	private bool isAvailable = true;
	private bool isActive = false;
	public float ticker = 5.0f;

	public bool shake = false;
	private float originalPostion;
	private SkinnedMeshRenderer rend;
	public Color[] colors;
	private CameraManager cm;

	void Awake(){
		rend = GetComponentInChildren<SkinnedMeshRenderer>();  
		colors = new Color[rend.materials.Length];	
		for (int i = 0; i < rend.materials.Length; i++) {
			colors [i] = rend.materials [i].color;
		}
	}

	void Update () {
		Vector3 camPostion;
		if (isActive) {
			ticker -= Time.deltaTime;
			camPostion = cm.mainCamera.transform.localPosition;
			float x = (originalPostion + Mathf.Sin(ticker * 8f));
			camPostion = new Vector3 (x, camPostion.y, camPostion.z);
			cm.mainCamera.transform.localPosition = camPostion;

			if (ticker < 0.0f) {
				isActive = false;
				camPostion = cm.mainCamera.transform.localPosition;
				camPostion.x = originalPostion;
				cm.mainCamera.transform.localPosition = camPostion;
				for (int i = 0; i < rend.materials.Length; i++) {
					rend.materials [i].color = colors [i];
				}
				print ("Disabled Elephant ability");
			} 
		}
	}

	public void applyAbility (){
		cm = FindObjectOfType<CameraManager> ();
		originalPostion = cm.mainCamera.transform.localPosition.x;
		if (isAvailable) {
			print ("Elephant Ability");
			isActive = true;
			foreach(Material m in rend.materials){
				m.color = Color.red;
			}

		}
		isAvailable = false;
	}
}
