using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	private PlayerDash pd;
	private Rigidbody rb;
	private Animator anim;

	private GameManager gm;

	private bool grounded;
	private bool knockedBack;
	private float knockBackTimer;

	public float bound = 15.0f;
	public float maxSpeed = 6.0f;
	public float minSpeed = 1.0f;
	public float speed = 6.0f;
	public float acceleration = 0.01f;
	public float dashSpeed = 2.0f;
	public float knockBackDelay = 1.0f;

	private float originalMass;

	public int playerNumber;

	// Raycast
	private RaycastHit hit;
	private Ray downRay;
	private Vector3 dir;

	void Start () {
		rb = GetComponent<Rigidbody> ();
		pd = GetComponent<PlayerDash> ();
		anim = GetComponentInChildren<Animator> ();

		gm = FindObjectOfType<GameManager> ();
		originalMass = rb.mass;
	}

	void FixedUpdate () {
		float h = Input.GetAxisRaw ("Horizontal " + playerNumber);
		float v = Input.GetAxisRaw ("Vertical " + playerNumber);

		if (grounded) {
			var movement = new Vector3 (h, 0, v);

			if (pd.isDashing) {
				movement *= dashSpeed;
			}

			// account for gravity
			movement.y = rb.velocity.y / speed;

			if(h != 0 || v != 0){
				speed = Mathf.Min(speed + acceleration, maxSpeed);
			}
			else{
				speed = Mathf.Max(speed - acceleration, minSpeed);
			}
				
			rb.velocity = movement * speed;

			if (h != 0 || v != 0) {
				transform.rotation = Quaternion.LookRotation (new Vector3 (movement.x, 0.0f, movement.z));
				anim.SetBool ("isMoving", true);
			} else {
				anim.SetBool ("isMoving", false);
			}
		}
	}

	void Update () {
		// manage mass
		if (gm.massUp) {
			rb.mass = originalMass * 2.0f;
		} else {
			rb.mass = originalMass;
		}

		if (knockedBack) {
			knockBackTimer -= Time.deltaTime;

			if (knockBackTimer <= 0.0f) {
				knockedBack = false;
				knockBackTimer = knockBackDelay; // reset timer
			}
		} else {
			CheckGrounded ();
		}
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.transform.tag == "Enemy") {
			BounceAway (collision.transform.position);
		}
	}

	public void BounceAway (Vector3 otherPos) {
		// Calculate vector away from collision object
		Vector3 awayDir = (transform.position - otherPos);

		// Calculate vector between direction and Y-axis (upwards)
		Vector3 dir = new Vector3 (awayDir.x, 0.0f, awayDir.z).normalized + new Vector3 (0, 1, 0);

		// Add impulse force in that direction
		rb.AddForce (dir * gm.activeBounceForce, ForceMode.Impulse);

		Debug.Log (playerNumber + " got bounced away at " + (dir * gm.activeBounceForce));

		grounded = false;
		knockedBack = true;
	}

	private void CheckGrounded () {
		downRay = new Ray (transform.position, Vector3.down);
		grounded = false;
		Debug.DrawRay (transform.position, Vector3.down);
			
		if (Physics.Raycast (downRay, out hit)) {
			if (hit.distance < 1.0f) {
				grounded = true;
			}
		}
	}

	public bool CheckInBounds () {
		float x = transform.position.x;
		float z = transform.position.z;

		if (x > -bound && x < bound && z > -bound && z < bound) {
			return true;
		}

		return false;
	}
}
