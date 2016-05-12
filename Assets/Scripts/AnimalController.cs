using UnityEngine;
using System.Collections;

public class AnimalController : MonoBehaviour {

	// Global References
	private GameManager gm;

	// Local References
	private AnimalDash pd;
	private Rigidbody rb;
	private Animator anim;

	// Control Variables
	public float bound = 15.0f;
	public float maxSpeed = 6.0f;
	public float minSpeed = 1.0f;
	public float acceleration = 0.01f;
	public float dashSpeed = 2.0f;
	public float knockBackDelay = 0.2f;

	// Management Variables
	private bool grounded;
	private float speed;
	private bool knockedBack;
	private float knockBackTimer;

	// Raycast Variables
	private RaycastHit hit;
	private Ray downRay;
	private Vector3 dir;

	void Start () {
		// Initialize Global References
		gm = FindObjectOfType<GameManager>();

		// Initialize Local References
		rb = GetComponent<Rigidbody> ();
		pd = GetComponent<AnimalDash> ();
		anim = GetComponentInChildren<Animator> (); // GetComponent<Animator>() when new fox is imported

		// Set initial variables
		speed = minSpeed;
	}

	void Update () {
		// Process knockback time
		if (knockedBack) {
			knockBackTimer -= Time.deltaTime;

			// Reset knockback
			if (knockBackTimer <= 0.0f) {
				knockedBack = false;
				knockBackTimer = knockBackDelay;
			}
		} else {
			CheckGrounded ();
		}
	}

	public void Move (float v, float h) {
		if (grounded) {
			// Create movement vector
			var movement = new Vector3 (h, 0, v);

			// Apply dashing speed
			if (pd.isDashing) {
				movement *= dashSpeed;
			}

			// Adjust Y movement to account for gravity
			movement.y = rb.velocity.y / speed;

			// Set moving speed
			if(h != 0 || v != 0){
				speed = Mathf.Min(speed + acceleration, maxSpeed);
			}

			// Set stationary speed
			else {
				speed = Mathf.Max(speed - acceleration, minSpeed);
			}

			// Apply changes in velocity
			rb.velocity = movement * speed;

			// If moving, look in the direction of movement
			if (h != 0 || v != 0) {
				transform.rotation = Quaternion.LookRotation (new Vector3 (movement.x, 0.0f, movement.z));
				anim.SetBool ("isMoving", true);
			} else {
				anim.SetBool ("isMoving", false);
			}
		}
	}

	void OnCollisionEnter (Collision collision) {
		// If this collides with another animal, bounce away
		if (collision.transform.tag == "Animal") {
			BounceAway (collision.transform.position);
		}
	}

	public void BounceAway (Vector3 otherPos) {
		// Calculate vector away from collision object
		Vector3 awayDir = (transform.position - otherPos);

		// Calculate vector between direction and Y-axis (upwards)
		Vector3 dir = new Vector3 (awayDir.x, 0.0f, awayDir.z).normalized + new Vector3 (0, 1, 0);

		// Add impulse force in that direction
		rb.AddForce (dir * gm.bounceForce, ForceMode.Impulse);

		// Allow player to leave the ground
		grounded = false;
		knockedBack = true;
	}

	private void CheckGrounded () {
		grounded = false;

		// Set up raycast variables
		downRay = new Ray (transform.position, Vector3.down);

		// Draw raycast ray [DEBUG]
		// Debug.DrawRay (transform.position, Vector3.down);

		// If ray hits anything within a distance of 1.0f, animal is grounded
		if (Physics.Raycast (downRay, out hit)) {
			if (hit.distance < 1.0f) {
				grounded = true;
			}
		}
	}

	public bool CheckInBounds () {
		float x = transform.position.x;
		float z = transform.position.z;

		// If animal is within the bounds, return true
		if (x > -bound && x < bound && z > -bound && z < bound) {
			return true;
		}

		// If animal is out of bounds, return false
		return false;
	}
}
