using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerDash : MonoBehaviour {
	public int maxDash = 20;
	public Text dashDisplay;

	public bool isDashing {
		get { return state == State.DASHING; }
	}

	private enum State { WAITING, DASHING, RECHARGING };

	private State state;
	private int dashRemaining;

	void Start() {
		state = State.WAITING;
		dashRemaining = maxDash;
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
		if (dashRemaining == maxDash && Input.GetKey(KeyCode.Space)) {
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
		if (dashRemaining == maxDash) {
			state = State.WAITING;
		} else {
			dashRemaining++;
		}
	}
}
