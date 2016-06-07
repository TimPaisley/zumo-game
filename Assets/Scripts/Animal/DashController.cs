using System;
using UnityEngine;

public class DashController : MonoBehaviour {
	public float dashSpeed;
	public float dashMass;
	public float dashLength;
	public float dashCooldown;

	public bool dashIsCharging { get; private set; }
	public bool isDashing { get; private set; }
	public float massMultiplier { get; private set; }

	public float currentDashCooldown {
		get { return dashCooldown * powerupController.dashCooldownMultiplier; }
	}

	private AudioSource dashSound;
	private PowerUpController powerupController;

	private float dashCharger;
	private float dashLengthRemaining;
	private float dashCooldownRemaining;

	public ParticleSystem ps;
	private ParticleSystem.EmissionModule em;

	void Awake() {
		massMultiplier = 1;
		dashSound = GetComponent<AudioSource>();
		powerupController = GetComponent<PowerUpController>();
		em = ps.emission;
	}

	void FixedUpdate() {
		if(dashIsCharging){
			dashCharger += Time.deltaTime;
			float cap = Mathf.Min(dashCharger, 4.0f);
			if(dashCharger > 5.0){
				dashCharger = 5;
			}
		} else if (isDashing) {
			dashLengthRemaining -= Time.deltaTime;
			if (dashLengthRemaining <= 0) {
				Stop();
				powerupController.displayDash(); //TODO differently
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
			return true;
		}

		return false;
	}

	public void PerformDash() {
		if (dashIsCharging) {
			dashIsCharging = false;
			massMultiplier = dashCharger;
			dashCharger = 0;

			isDashing = true;
			dashLengthRemaining = dashLength;

			dashSound.PlayOneShot(dashSound.clip);
		}
	}

	public void Stop() {
		em.enabled = false;
		ps.Stop ();
		isDashing = false;
		dashIsCharging = false;

		massMultiplier = 1;
		dashLengthRemaining = 0;
		dashCharger = 0;
		dashCooldownRemaining = currentDashCooldown;
	}
}
