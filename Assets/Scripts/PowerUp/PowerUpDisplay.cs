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
	private Rigidbody animalRB;

	private RectTransform canvasRect;
	private RectTransform rectTransform;
	private Text text;
	private Dictionary <string,GameObject> powerUps;
	private ArrayList powerUpQueue;

    void Awake () {
		powerUps = new Dictionary<string, GameObject> ();
		powerUpQueue = new ArrayList();
    }

	void Start(){
        Display = Instantiate(PuDisplayPrefab);
		canvasRect = canvas.GetComponent<RectTransform>();
		rectTransform = Display.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasRect, false);
        animal = GetComponent<AnimalController>();
		animalRB = GetComponent<Rigidbody> ();
        cameraManager = FindObjectOfType<CameraManager>();
	}
		
	void Update () {
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
				animal.removePowerUp(ph.getPuType());
				index = i;     
			}
			updateTimer(ph.getPuType(),currTicker);
		}

		if(index != -1)
		{
			powerUpQueue.RemoveAt(index);
		}
	}

	void OnCollisionEnter (Collision collision) {
		// If the collides with power up item
		if (collision.transform.tag == "PowerUp")
		{
			PowerUp pu = collision.gameObject.GetComponent<PowerUp>();
			string currentPower = pu.getPowerUpType();
			PowerUpHistory nph = new PowerUpHistory(currentPower);
			for(int i = 0; i<powerUpQueue.Count; i++)
				// if the power up is already active on the animal restart the timer
			{
				PowerUpHistory ph = (PowerUpHistory)powerUpQueue[i];
				if (ph.getPuType().Equals(currentPower))
				{
					//Debug.LogWarning("Extended: " + currentPower + " timer");
					ph.restTicker();
					Destroy(collision.gameObject);
					displayPowerUp(currentPower);
					return;
				}
			}
			powerUpQueue.Add(nph);
			// Debug.LogWarning("Apply: " + currentPower + " powerup");
			if (currentPower.Equals("mass"))
			{
				this.transform.localScale = (this.transform.localScale * 1.5f);

				animalRB.mass = animalRB.mass * pu.getMassMultiplie();
			}
			else if (currentPower.Equals("speed"))
			{
				animal.minSpeed *= pu.getSpeedMulti();
				animal.maxSpeed *= pu.getSpeedMulti();
				animal.speed = animal.maxSpeed;
			}
			else if (currentPower.Equals("dashCD"))
			{
				animal.dashCooldown = (float)(animal.dashCooldown/pu.getNewDashCD());
			}
			Destroy(collision.gameObject);
			displayPowerUp(currentPower);
		}

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
			} else if (name.Equals ("dashCD")) {
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

	public class PowerUpHistory{
		private string pu = "";
		private float ticker = 10.0f;

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
			ticker = 10.0f;
		}
		public string getPuType()
		{
			return pu;
		}
	}
}
