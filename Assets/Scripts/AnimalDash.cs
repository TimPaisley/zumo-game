using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimalDash : MonoBehaviour {

	// Global References
	private PlayerController playerController;

	// Control Variables
	public int cooldown = 50;
	public int dashLength = 10;

	// Management Variables
	public Text dashDisplay;
	public bool isDashing { get { return state == State.DASHING; }}
	public int dashRemaining { get; private set; }
	private enum State { WAITING, DASHING, RECHARGING };
	private State state;

	// Temporary Variables
	public int tempPlayer;

	void Start() {
		state = State.WAITING;
		dashRemaining = dashLength;
		playerController = GetComponent<PlayerController>();
	}

	void FixedUpdate() {
		if (state == State.WAITING) keepWaiting();
		else if (state == State.DASHING) keepDashing();
		else keepRecharging();
	}

	void Update() {
		if (dashDisplay) dashDisplay.text = "Dash: " + dashRemaining.ToString();
	}

	private void keepWaiting() {
		if (Input.GetButton("Dash " + tempPlayer)) {
			state = State.DASHING;
		}
	}

	private void keepDashing() {
		if (dashRemaining == 0) {
			state = State.RECHARGING;
		} else {
			dashRemaining--;
		}
	}

	private void keepRecharging() {
		if (dashRemaining == cooldown) {
			dashRemaining = dashLength;
			state = State.WAITING;
		} else {
			dashRemaining++;
		}
	}
}
