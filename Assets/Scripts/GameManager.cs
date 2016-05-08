using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	public GameObject pandaPrefab;
	public GameObject pickupPrefab;

	public GameObject[] pickUpSpots;
	public GameObject[] pandaDropSpots;

	public bool testing;

	public float bounceForce = 10.0f;
	public float pickUpDelay = 10.0f;
	public float pandaDropDelay = 5.0f;
	public float effectDuration = 5.0f;

	[HideInInspector]
	public float activeBounceForce;
	private float pickTime;
	private float effectTime;

	private Stack inv;
	private int score = 0;

	private PickUp activePickup;
	private string[] pickUpEffects = {
		"BounceUp", "MassUp", "SpeedUp"
	};

	public bool bounceUp;
	public bool massUp;
	public bool speedUp;

	void Start () {
		inv = new Stack ();

		activeBounceForce = bounceForce;
		pickTime = pickUpDelay;
		effectTime = effectDuration;
	}

	void Update () {
		// manage player input
		if (Input.GetButtonDown ("Jump") && inv.Count > 0) {
			string peek = (string) inv.Peek ();

			// consumables
			if (peek != "" && !bounceUp && !massUp && !speedUp) {
				string effect = PopFromInventory ();

				if (effect == "BounceUp") {
					Debug.Log ("Bounce Force increased for 5.0s");
					bounceUp = true;
				} else if (effect == "MassUp") {
					Debug.Log ("Player Mass increased for 5.0s");
					massUp = true;
				} else if (effect == "SpeedUp") {
					Debug.Log ("Player Speed increased for 5.0s");
					speedUp = true;
				}
			}

			// throwables
			else if (peek == "") {

			}
		}

		// manage bounce force
		if (bounceUp) {
			activeBounceForce = bounceForce * 2.0f;
		} else {
			activeBounceForce = bounceForce;
		}

		// manage timers
		pickTime -= Time.deltaTime;

		if (bounceUp || massUp || speedUp) {
			effectTime -= Time.deltaTime;
		}

		// manage timer tickovers
		if (pickTime <= 0.0f) {
			pickTime = pickUpDelay;
			//Debug.Log ("Deploying Pick Up");
			DeployPickUp ();
		}

		if (effectTime < 0.0f) {
			effectTime = effectDuration;
			Debug.Log ("The effect wore off...");
			//cm.UpdateSlots (inv);
			ResetEffects ();
		}
	}

	public void PushToInventory(string effect) {
		if (inv.Count < 5 && !bounceUp && !massUp && !speedUp) {
			Debug.Log (effect + " added to Inventory");
			inv.Push (effect);
			//cm.UpdateSlots (inv);
		} else {
			Debug.Log ("Inventory is Full");
		}
	}

	public string PopFromInventory() {
		return (string) inv.Pop ();
	}

	private void ResetEffects () {
		bounceUp = false;
		massUp = false;
		speedUp = false;
	}

	public void AddScore () {
		score++;
	}

	private void DeployPickUp () {
		// destroy active pickup
		if (activePickup != null) {
			Destroy (activePickup.gameObject);
		}

		// assign random position and random effect of new pickup
		int pos = Mathf.FloorToInt(Random.value * pickUpSpots.Length);
		int effect = Mathf.FloorToInt(Random.value * pickUpEffects.Length);

		// deploy new pickup
		GameObject newPickUp = (GameObject) Instantiate(pickupPrefab, pickUpSpots[pos].gameObject.transform.position, Quaternion.identity);
		activePickup = newPickUp.GetComponent<PickUp> (); // convert from gameObject to PickUp
		activePickup.SetEffect (pickUpEffects[effect]);
	}
}
