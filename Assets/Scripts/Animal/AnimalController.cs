﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(DashController), typeof(PowerUpController))]
public class AnimalController : MonoBehaviour {
	private const float ALMOST_ZERO = 0.15f;

	// Global References
	private GameManager gm;

	// Local References
	private Rigidbody rb;
	private Animator anim;
	private DashController dashController;
	private PowerUpController powerupController;

	// Control Variables
	public Renderer board;

	[Header("Movement")]
	public float minSpeed = 1.0f;
	public float maxSpeed = 6.0f;
	public float minTurnSpeed = 6.0f;
	public float acceleration = 0.01f;
	public float decceleration = 0.1f;
	[Header("Turning")]
	public float slowAngle;
	public float turnRate = 5.0f;
	[Header("Other")]
	public float knockBackDelay = 0.2f;

	// Management Variables
	private float baseMass;
	private bool knockedBack;
	private float knockBackTimer;
	private int stationaryDelay = 0;

	// Raycast Variables
	private RaycastHit hit;
	private Ray downRay;

	//Ability related fields
	//If panda ability is active, this is used due to how mass is handled currently
	public bool pandaAbility = false; 
	public bool disableControl = false;

	public bool isDashing {
		get { return dashController.isDashing; }
	}
	public bool isDashCharging {
		get { return dashController.dashIsCharging; }
	}
	public float currentMaxSpeed {
		get { return maxSpeed * powerupController.speedMultiplier; }
	}
	public float currentMass {
		get { return baseMass * powerupController.massMultiplier * dashController.massMultiplier; }
	}

	public float speed { get; private set; }

	public bool isInBounds {
		get {
			// Check whether a raycast straight down hits the ground
			var raycastHit = raycast(new Vector3 (transform.position.x, transform.position.y + 0.5f, transform.position.z), Vector3.down);

			return raycastHit.HasValue && Tags.HasAnyTag(raycastHit.Value.collider.gameObject, Tags.BoardObjects);
		}
	}

	void Start() {
		// Initialize Global References
		gm = FindObjectOfType<GameManager>();

		// Initialize Local References
		rb = GetComponent<Rigidbody>();
		anim = GetComponentInChildren<Animator>(); // GetComponent<Animator>() when new fox is imported

		dashController = GetComponent<DashController>();
		powerupController = GetComponent<PowerUpController>();

		baseMass = rb.mass;
		speed = minSpeed;
	}

	void Update() {
		// Make sure mass is kept up to date
		if (pandaAbility) {
			//The panda's ability is implemented here because the current 
			//implementation updates mass every frame
			//TODO Move the ability out // might require to change how mass is handled
			rb.mass = 1000;
		} else {
			rb.mass = currentMass;
		}

		// Update animator
		if (rb.velocity.magnitude > ALMOST_ZERO || dashController.dashIsCharging) {
			anim.SetBool("isMoving", true);
			anim.speed = Mathf.Max(speed / 10, 0.7f);
		} else {
			anim.SetBool("isMoving", false);
		}

		// Process knockback time
		if (knockedBack) {
			knockBackTimer -= Time.deltaTime;

			// Reset knockback
			if (knockBackTimer <= 0.0f) {
				knockedBack = false;
				knockBackTimer = knockBackDelay;
			}
		}
	}

	void OnTriggerEnter(Collider other) {
		// If this collides with another animal, bounce away and display particle
		if (!knockedBack && other.transform.tag == "AnimalHead") {
			dashController.Stop();
			// StartCoroutine(gm.ShowCollisionParticle(other.contacts [0].point));

			// Calculate vector away from collision object
			Vector3 awayDir = (transform.position - other.transform.parent.position);

			// Calculate vector between direction and Y-axis (upwards)
			Vector3 dir = new Vector3 (awayDir.x, 0.0f, awayDir.z).normalized + new Vector3 (0, 1, 0);

			Debug.Log ("Trigger Collision");

			// Add impulse force in that direction
			rb.AddForce(dir * other.gameObject.GetComponentInParent<Rigidbody>().velocity.magnitude/2 * gm.bounceForce, ForceMode.Impulse);

			// Allow player to leave the ground
			knockedBack = true;
		}
	}

	private bool isGrounded {
		get {
			var raycastHit = raycast(new Vector3 (transform.position.x, transform.position.y + 0.5f, transform.position.z), Vector3.down);

			return raycastHit.HasValue && raycastHit.Value.distance < 1.0f;
		}
	}

	void OnCollisionEnter(Collision collision) {
		//if it collides with terrain
		if (collision.transform.tag == "Environment") {
			dashController.Stop();
		}

		//if it collides with powerup
		if (collision.transform.tag == "PowerUp") {
			powerupController.Apply(collision.gameObject.GetComponent<PowerUp>());
			Destroy(collision.gameObject);
		}
	}

	public void Rotate(float v, float h) {
		if (!disableControl) { // Disable control when actived panda ability
			
			if (v == 0 && h == 0) {
				return;
			}

			if (!knockedBack && isGrounded) {
				var movement = new Vector3 (h, 0, v);

				//rotate speed is dependent on current speed
				transform.rotation = Quaternion.Lerp (transform.rotation, Quaternion.LookRotation (new Vector3 (movement.x, 0.0f, movement.z)), Time.deltaTime * (turnRate / speed));

				//deccelerate if turning
				if (Vector3.Angle (movement, transform.forward) > slowAngle && speed > minTurnSpeed) {
					speed = Mathf.Max (speed - decceleration, minSpeed);
				}
			}
		}
	}

	public void Move(float inputMagnitude) {
		if (!disableControl) { // Disable control when actived panda ability
			
			if (inputMagnitude == 0) {
				return;
			}

			var maxMovementSpeed = currentMaxSpeed * inputMagnitude;
			var movement = new Vector3 (transform.forward.x, rb.velocity.y, transform.forward.z);

			if (!knockedBack && isGrounded) {
				// Adjust Y movement to account for gravity
				movement.y = rb.velocity.y / speed;

				// Apply changes in velocity
				if (isDashing) {
					// Make sure player doesn't fly upwards on slopes
					movement.y = Mathf.Clamp (movement.y, 0, 0.01f);

					rb.velocity = movement * dashController.dashSpeed;
				} else {
					speed = Mathf.Min (speed + acceleration, maxMovementSpeed);

					rb.velocity = movement * speed;
				}

				//remove speed if player is stationary
				if (rb.velocity == Vector3.zero) {
					stationaryDelay++;
					if (stationaryDelay == 5) {
						speed = minSpeed;
					}
				} else {
					stationaryDelay = 0;
				}
			}
		}
	}

	public void StartDashCharge() {
		if (dashController.StartDashCharge()) {
			rb.velocity = Vector3.zero;
		}
	}

	public void PerformDash() {
		if (!disableControl) { // Disable control when actived panda ability
			dashController.PerformDash();
		}
	}

	public void PerformAbility(){
		AnimalAbility ability = GetComponent<AnimalAbility>();
		ability.applyAbility ();
	}

	public void Kill() {
		anim.Stop();
		throwOutOfBounds();
	}

	private RaycastHit? raycast(Vector3 origin, Vector3 direction) {
		// Set up raycast variables
		downRay = new Ray (origin, direction);

		// Draw raycast ray [DEBUG]
		Debug.DrawRay(origin, direction);
		
		if (Physics.Raycast(downRay, out hit)) {
			return hit;
		} else {
			return null;
		}
	}

	private void throwOutOfBounds() {
		var boardPos = board.transform.position;

		rb.freezeRotation = false;
		rb.mass = 10;
		rb.AddForce(Vector3.Normalize(transform.position - boardPos) * 100 + Vector3.up * 200);
		rb.AddTorque(1000, 500, 1000);
	}
}