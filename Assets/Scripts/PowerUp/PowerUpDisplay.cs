using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PowerUpDisplay : MonoBehaviour {
	//Prefab
	public GameObject PuDisplayPrefab;
	public GameObject MassPuPrefab;
	public GameObject SpeedPuPrefab;
	public GameObject dashPrefab;
	public Transform initialPostion;
	//Canvas
	public Canvas canvas;

	//object reference
	private GameObject Display;
	private float puDuration = 10f;
	//private PlayerController player;
	public Camera worldCamera;
	private int yOffset = 40;
	private AnimalController animal;
	private RectTransform canvasRect;
	private RectTransform rectTransform;
	private Text text;
	private Dictionary <string,GameObject> powerUps;

    void Awake () {
		powerUps = new Dictionary<string, GameObject> ();
    }

	void Start(){
		//Display = Instantiate(PuDisplayPrefab);
		//Display.transform.SetParent (canvas.transform);
		animal = GetComponent<AnimalController>();
		canvasRect = Display.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
		rectTransform = Display.GetComponent<RectTransform>();
	}
		
	void Update () {
		var animalPos = worldCamera.WorldToViewportPoint(animal.transform.position);
		var screenPos = new Vector2(
			((animalPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
			((animalPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f))
		);
		screenPos.y += yOffset + (yOffset / 30) * Mathf.Sin(Time.fixedTime * 5);

		rectTransform.anchoredPosition = screenPos;

	}

	public void displayPowerUp(string name){
		if (!powerUps.ContainsKey (name)) {
			GameObject pu;
			if (name.Equals ("mass")) {
				pu = Instantiate (MassPuPrefab);
				pu.transform.SetParent (Display.transform);
				powerUps.Add (name, pu);
			} else if (name.Equals ("speed")) {
				pu = Instantiate (SpeedPuPrefab);
				pu.transform.SetParent (Display.transform);
				powerUps.Add (name, pu);
			} else if (name.Equals ("dash")) {
				pu = Instantiate (dashPrefab);
				pu.transform.SetParent (Display.transform);
				powerUps.Add (name, pu);
			}
		} else {
			GameObject renew = powerUps [name];
			renew.transform.GetChild (1).GetComponent<Image> ().fillAmount = 1f;
		}
	}

	public void updateTimer(string name, float remain){
		if (powerUps.ContainsKey (name)) {
			GameObject renew = powerUps [name];
			float remainDur = remain/puDuration;
			renew.transform.GetChild (1).GetComponent<Image> ().fillAmount = remainDur;
			if (remain < 0.0f) {
				Destroy (renew);
				powerUps.Remove (name);
			}
		}
	}
	public void updateDashTimer(float remain){
		if (powerUps.ContainsKey ("dash")) {
			GameObject renew = powerUps ["dash"];
			float remainDur = remain/1.0f;
			renew.transform.GetChild (1).GetComponent<Image> ().fillAmount = remainDur;
			if (remain < 0.0f) {
				Destroy (renew);
				powerUps.Remove ("dash");
			}
		}
	}
}
