using System;
using UnityEngine;

public class DashController : MonoBehaviour {
	public float dashSpeed;
	public float dashMass;
	public float dashLength;
	public float dashCooldown;


	public bool dashIsCharging { get; private set; }
	public bool isDashing { get;  set; }
	public float massMultiplier { get; private set; }

	public float currentDashCooldown {
		get { return dashCooldown * powerupController.dashCooldownMultiplier; }
	}
	public AudioSource chargeSound;
	private AudioSource[] dashSound;
	private PowerUpController powerupController;

	private float dashCharger;
	private float dashLengthRemaining;
	private float dashCooldownRemaining;
	private bool charged = false;
	private bool minCharge = false;

	public ParticleSystem ps;
	private ParticleSystem.EmissionModule em;

	void Awake() {
		massMultiplier = 1;
		dashSound = GetComponents<AudioSource>();
		powerupController = GetComponent<PowerUpController>();
		//set up hitSound
		chargeSound.ignoreListenerVolume = true;

		em = ps.emission;

	}

	void FixedUpdate() {
		
		if(dashIsCharging){
			dashCharger += Time.deltaTime;
			chargeSound.volume = (0.5f)*dashCharger;

			if(dashCharger > 5.0&&charged==false){
				chargeSound.Stop ();
				charged=true;
				dashCharger = 5;

			}
		} else if (isDashing) {
			dashLengthRemaining -= Time.deltaTime;
			if (dashLengthRemaining <= 0) {
				Stop();
				GetComponentInParent<AnimalController> ().halt();
				//moved powerupcode to Stop();
			} 
		} else {
			dashCooldownRemaining -= Time.deltaTime;
			powerupController.updateDashTimer(dashCooldownRemaining / dashCooldown); //TODO

			if (dashCooldownRemaining < 0.0f) {
				dashCooldownRemaining = 0;
			}
		}
	}

	public bool StartDashCharge() {
		if (dashCooldownRemaining == 0) {
			ps.Simulate(0.0f,true,true);
			em.enabled = true;
			ps.Play ();

			dashIsCharging = true;
			chargeSound.volume = 1.0f;
			chargeSound.Play();
			return true;
		}

		return false;
	}

	public void PerformDash() {

		if (dashIsCharging) {
			dashIsCharging = false;
			massMultiplier = Mathf.Max(dashCharger,1.0f);
			isDashing = true;
			dashLengthRemaining = dashLength;
			dashCharger = 0;

			int index = (int)(3*UnityEngine.Random.value);
			dashSound[index].PlayOneShot(dashSound[index].clip);
		}
	}

	public void Stop() {
		em.enabled = false;
		ps.Stop ();
		isDashing = false;
		dashIsCharging = false;
		charged = false;
		powerupController.displayDash(); //TODO differently

		chargeSound.Stop ();
		massMultiplier = 1;
		dashLengthRemaining = 0;
		dashCharger = 0;
		dashCooldownRemaining = currentDashCooldown;

	}
}
