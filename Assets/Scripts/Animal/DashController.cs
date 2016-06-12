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

	public ParticleSystem chargePS;
	public ParticleSystem dashPS;
	private ParticleSystem.EmissionModule chargeEM;
	private ParticleSystem.EmissionModule dashEM;

	void Awake() {
		massMultiplier = 1;
		dashSound = GetComponents<AudioSource>();
		powerupController = GetComponent<PowerUpController>();
		//set up hitSound
		chargeSound.ignoreListenerVolume = true;

		chargeEM = chargePS.emission;
		dashEM = dashPS.emission;
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
			chargePS.Simulate(0.0f,true,true);
			chargeEM.enabled = true;
			chargePS.Play ();

			dashIsCharging = true;
			chargeSound.volume = 1.0f;
			chargeSound.Play();
			return true;
		}

		return false;
	}

	public void PerformDash() {

		if (dashIsCharging) {
			chargeEM.enabled = false;
			chargePS.Stop ();
			chargePS.Clear ();

			dashPS.Simulate(0.0f,true,true);
			dashEM.enabled = true;
			dashPS.Play ();

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
		dashEM.enabled = false;
		dashPS.Stop ();
		dashPS.Clear ();

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
