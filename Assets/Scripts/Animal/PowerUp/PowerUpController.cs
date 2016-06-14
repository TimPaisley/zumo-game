using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PowerUpController : MonoBehaviour {
	//Prefab
	public GameObject PuDisplayPrefab;
	public GameObject MassPuPrefab;
	public GameObject SpeedPuPrefab;
	public GameObject dashPrefab;
	public GameObject dashCDPrefab;
	public Transform initialPostion;
	//Canvas
	public Canvas canvas;

	//object reference
	private GameObject Display;
	private float puDuration = 10f;
    private CameraManager cameraManager;
	private int yOffset = 100;
	private AnimalController animal;
	//private Rigidbody animalRB;

	private RectTransform canvasRect;
	private RectTransform rectTransform;
	private Text text;
	private Dictionary <string,GameObject> powerUps;
	private List<PowerUpHistory> powerUpQueue;

	public float speedMultiplier { get; private set; }
	public float massMultiplier { get; private set; }
	public float dashCooldownMultiplier { get; private set; }

	private Renderer rend;

    void Awake () {
		powerUps = new Dictionary<string, GameObject> ();
		powerUpQueue = new List<PowerUpHistory> ();

		speedMultiplier = 1;
		massMultiplier = 1;
		dashCooldownMultiplier = 1;

        Display = Instantiate(PuDisplayPrefab);
		canvasRect = canvas.GetComponent<RectTransform>();
		rectTransform = Display.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasRect, false);
        animal = GetComponent<AnimalController>();
        cameraManager = FindObjectOfType<CameraManager>();
	}

	void Update () {
		if (animal == null) {
			return;
		}

		var animalPos = cameraManager.mainCamera.WorldToViewportPoint(animal.transform.position);
		var screenPos = new Vector2(
			((animalPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
			((animalPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f))
		);
		screenPos.y += yOffset + (yOffset / 30) * Mathf.Sin(Time.fixedTime * 5);

		rectTransform.anchoredPosition = screenPos;

		int index = -1; 
		// The index of the power up that needs to be removed
		for (int i = 0; i < powerUpQueue.Count; i++)
			//Check if powerup run out of time
		{
			PowerUpHistory ph = (PowerUpHistory)powerUpQueue[i];
			float currTicker = ph.getTicker();
			if (currTicker < 0.0f)
			{
				removePowerUp(ph.getPuType());
				index = i;     
			}
			updateTimer(ph.getPuType(),currTicker);
		}

		if(index != -1)
		{
			powerUpQueue.RemoveAt(index);
		}
	}

	public void RemoveAll () {
		foreach (var powerup in powerUps) {
			removePowerUp(powerup.Key);

			Destroy(powerup.Value.gameObject);
		}

		powerUps.Clear();
		powerUpQueue.Clear();
	}

	public void Apply(PowerUp pu) {
		string currentPower = pu.getPowerUpType();
		PowerUpHistory nph = new PowerUpHistory(currentPower);

		foreach (var ph in powerUpQueue) {
			// if the power up is already active on the animal restart the timer
			if (ph.getPuType().Equals(currentPower)) {
				//Debug.LogWarning("Extended: " + currentPower + " timer");
				ph.restTicker();
				displayPowerUp(currentPower);
				return;
			}
		}
		powerUpQueue.Add(nph);

		if (currentPower.Equals("mass")) {
			this.transform.localScale = (this.transform.localScale * 1.5f);

			massMultiplier = pu.getMassMultiplie();
		}
		else if (currentPower.Equals("speed")) {
			speedMultiplier = pu.getSpeedMulti();
		}
		else if (currentPower.Equals("stop")) {
			nph.setTicker(2.0f);
			animal.stopPowerup = true;
			AnimalController[] acs = FindObjectsOfType<AnimalController> ();
			print (acs.Length);
			foreach (AnimalController ac in acs) {
				if (!ac.stopPowerup) {
					ac.disableControl = true;
				}
			}
			//dashCooldownMultiplier = pu.getDashCDMulti();
		}

		displayPowerUp(currentPower);
	}

	public void removePowerUp(string currentPower) {
		//Debug.LogWarning("Remove: "+ currentPower +" change");
		if (currentPower.Equals("mass")) {
			this.transform.localScale = (this.transform.localScale / 1.5f);
			massMultiplier = 1;
		} else if (currentPower.Equals("speed")) {
			speedMultiplier = 1;
		} else if (currentPower.Equals("stop")) {
			//dashCooldownMultiplier = 1;
			animal.stopPowerup = false;
			AnimalController[] acs = FindObjectsOfType<AnimalController> ();
			foreach (AnimalController ac in acs) {
				ac.disableControl = false;
			}
		}
	}

	private void displayPowerUp(string name){
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
			} else if (name.Equals ("stop")) {
				pu = Instantiate (dashCDPrefab);
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
			if (name.Equals ("stop")) {
				remainDur = remain / 2.0f;
			} 
			renew.transform.GetChild (1).GetComponent<Image> ().fillAmount = remainDur;
			if (remain < 0.0f) {
				Destroy (renew);
				powerUps.Remove (name);
			}
		}
	}

	public void displayDash() {
		//TODO differently
		displayPowerUp("dash");
	}

	public void updateDashTimer(float remain){
		if (powerUps.ContainsKey ("dash")) {
			GameObject renew = powerUps ["dash"];
			renew.transform.GetChild (1).GetComponent<Image> ().fillAmount = remain;
			if (remain < 0.0f) {
				Destroy (renew);
				powerUps.Remove ("dash");
			}
		}
	}

	public class PowerUpHistory{
		private string pu = "";
		private float ticker = 10.0f;
		private float original = 10.0f;
		public PowerUpHistory(string p)
		{
			pu = p;
		}
		public float getTicker()
		{
			ticker -= Time.deltaTime;
			return ticker;
		}
		public void restTicker()
		{
			ticker = original;
		}
		public string getPuType()
		{
			return pu;
		}

		public void setTicker(float time){
			ticker = time;
			original = time;
		}
	}
}
