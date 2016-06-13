using UnityEngine;
using System.Collections;

public class PantherAbility : MonoBehaviour,AnimalAbility {

	private bool isAvailable = true;
	private bool isActive = false;
	public float ticker = 5.0f;

	private float originalPostion;
	private CameraManager cm;
	public Camera blackscreen;

	void Update () {
		Vector3 camPostion;
		if (isActive) {
			ticker -= Time.deltaTime;
			if (ticker < 0.0f) {
				isActive = false;
				blackscreen.gameObject.SetActive (false);
				print ("Disabled Panther ability");
			} 
		}
	}

	public void applyAbility (){
		cm = FindObjectOfType<CameraManager> ();
		originalPostion = cm.mainCamera.transform.localPosition.x;
		if (isAvailable) {
			print ("Panther Ability");
			isActive = true;
			blackscreen.gameObject.SetActive (true);
		}
		isAvailable = false;
	}
}
