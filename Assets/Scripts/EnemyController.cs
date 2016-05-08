using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

	private Rigidbody rb;

	private GameManager gm;
	private PlayerController player;

	private bool grounded;
	private bool knockedBack;
	private float knockBackTimer;

	public float speed = 6.0f;
	public float bound = 15.0f;
	public float knockBackDelay = 1.0f;

	// Raycast
	private RaycastHit hit;
	private Ray downRay;
	private Vector3 dir;

	void Start () {
		rb = GetComponent<Rigidbody> ();

		gm = FindObjectOfType<GameManager> ();
		player = FindObjectOfType<PlayerController> ();
	}

	void FixedUpdate () {
		if (player.CheckInBounds () && grounded) {
			Vector3 dir = (player.transform.position - this.transform.position).normalized;
			rb.velocity = new Vector3 (dir.x, rb.velocity.y/speed, dir.z) * speed;
			transform.rotation = Quaternion.LookRotation (new Vector3 (dir.x, 0.0f, dir.z));
		}
	}

	void Update () {
		if (knockedBack) {
			knockBackTimer -= Time.deltaTime;

			if (knockBackTimer <= 0.0f) {
				knockedBack = false;
				knockBackTimer = knockBackDelay; // reset timer
			}
		} else {
			CheckGrounded ();
		}

		CheckKnockedOut ();
	}

	void OnCollisionEnter (Collision collision) {
		if (collision.transform.tag == "Player" || collision.transform.tag == "Environment") {
			BounceAway (collision.transform.position);
		}
	}

	public void BounceAway (Vector3 otherPos) {
		// Calculate vector away from collision object
		Vector3 awayDir = (transform.position - otherPos);

		// Calculate vector between direction and Y-axis (upwards)
		Vector3 dir = new Vector3 (awayDir.x, 0.0f, awayDir.z) + new Vector3 (0, 1, 0);

		// Add impulse force in that direction
		rb.AddForce (dir * gm.activeBounceForce, ForceMode.Impulse);

		grounded = false;
		knockedBack = true;
	}

	private void CheckGrounded () {
		downRay = new Ray (transform.position, Vector3.down);
		grounded = false;

		if (Physics.Raycast (downRay, out hit)) {
			if (hit.collider.gameObject.tag == "Ground" && hit.distance < 0.1f) {
				grounded = true;
			}
		}
	}

	private void CheckKnockedOut () {
		if (transform.position.y < -10.0f) {
			gm.AddScore ();
			Destroy (this.gameObject);
		}
	}
}
