using UnityEngine;
using System.Collections;

public class BombController : MonoBehaviour {

	private GameManager gm;

	private Rigidbody rb;
	private MeshRenderer[] mr;

	private Vector3 originalPosition;

	public ParticleSystem fusePS;
	public ParticleSystem detonatePS;
	private ParticleSystem.EmissionModule fuseEM;
	private ParticleSystem.EmissionModule detonateEM;

	public AudioSource bombTick;
	public AudioSource bombExplosion;

	public float fuseTimer = 10.0f;
	public float bombPower = 200.0f;

	private bool deployed = false;

	void Start () {
		gm = FindObjectOfType<GameManager> ();

		mr = GetComponentsInChildren<MeshRenderer> ();
		rb = GetComponent<Rigidbody> ();


		for (int i = 0; i < mr.Length; i++) {
			mr[i].enabled = false;
		}

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
		if (deployed) {
			return;
		}

		for (int i = 0; i < mr.Length; i++) {
			mr[i].enabled = true;
		}

		rb.useGravity = true;
		bombTick.Play ();
		StartCoroutine (Countdown ());
	}

	private IEnumerator Countdown () {
		fusePS.Simulate(0.0f,true,true);
		fuseEM.enabled = true;
		fusePS.Play ();

		while (fuseTimer > 0) {
			yield return new WaitForSeconds (1);
			fuseTimer--;
		}

		StartCoroutine(Detonate ());
	}

	private IEnumerator Detonate () {
		fuseEM.enabled = false;
		fusePS.Stop ();
		fusePS.Clear ();

		detonatePS.Simulate(0.0f,true,true);
		detonateEM.enabled = true;
		detonatePS.Play ();

		bombTick.Stop ();
		bombExplosion.Play ();

		gm.ApplyBombForce (this.transform.position, bombPower);

		for (int i = 0; i < mr.Length; i++) {
			mr[i].enabled = false;
		}

		yield return new WaitForSeconds (1);
		Reset ();
	}

	public void Reset () {
		fuseEM.enabled = false;
		fusePS.Stop ();
		fusePS.Clear ();

		for (int i = 0; i < mr.Length; i++) {
			mr[i].enabled = false;
		}

		transform.eulerAngles = Vector3.zero;
		rb.velocity = Vector3.zero;

		fuseTimer = 10.0f;
		rb.useGravity = false;

		transform.position = originalPosition;
	}
}
