using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerDash : MonoBehaviour {
	public int cooldown = 50;
	public int dashLength = 10;
	public Text dashDisplay;

	public bool isDashing {
		get { return state == State.DASHING; }
	}
	public int dashRemaining { get; private set; }

	private enum State { WAITING, DASHING, RECHARGING };

	private State state;
	private PlayerController playerController;

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
		if (Input.GetButton("Dash " + playerController.playerNumber)) {
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
