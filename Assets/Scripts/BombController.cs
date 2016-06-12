using UnityEngine;
using System.Collections;

public class BombController : MonoBehaviour {

	private GameManager gm;

	private Rigidbody rb;
	private MeshRenderer mr;

	private Vector3 originalPosition;

	public ParticleSystem fusePS;
	public ParticleSystem detonatePS;
	private ParticleSystem.EmissionModule fuseEM;
	private ParticleSystem.EmissionModule detonateEM;

	public float fuseTimer = 10.0f;
	public float bombPower = 200.0f;

	void Start () {
		gm = FindObjectOfType<GameManager> ();

		mr = GetComponentInChildren<MeshRenderer> ();
		rb = GetComponent<Rigidbody> ();

		mr.enabled = false;
		rb.useGravity = false;

		originalPosition = transform.position;

		fuseEM = fusePS.emission;
		detonateEM = detonatePS.emission;
	}

	void Update () {
		// Test Deploy
		if (Input.GetKeyDown (KeyCode.B)) {
			Debug.Log ("DEPLOYING");
			Deploy ();
		}
	}

	public void Deploy () {
		mr.enabled = true;
		rb.useGravity = true;

		StartCoroutine (Countdown ());
	}

	private IEnumerator Countdown () {
		fusePS.Simulate(0.0f,true,true);
		fuseEM.enabled = true;
		fusePS.Play ();

		while (fuseTimer > 0) {
			Debug.Log (fuseTimer + " seconds until detonation...");
			yield return new WaitForSeconds (1);
			fuseTimer--;
		}

		Detonate ();
	}

	private void Detonate () {
		fuseEM.enabled = false;
		fusePS.Stop ();
		fusePS.Clear ();

		detonatePS.Simulate(0.0f,true,true);
		detonateEM.enabled = true;
		detonatePS.Play ();

		Debug.Log ("BOOOOOOM!!!");
		gm.ApplyBombForce (this.transform.position, bombPower);
		Reset ();
	}

	public void Reset () {
		fuseEM.enabled = false;
		fusePS.Stop ();
		fusePS.Clear ();

		Debug.Log ("Bomb Reset");
		mr.enabled = false;
		rb.useGravity = false;

		transform.position = originalPosition;
	}
}
