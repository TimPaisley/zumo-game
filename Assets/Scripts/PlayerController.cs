using UnityEngine;
using InControl;
using System.Collections;

public class PlayerController : MonoBehaviour {
	// Global References


	// Player Variables
	public int playerNumber;
	public AnimalController animal;

	private InputDevice input;

	void Start () {
		input = InputManager.Devices[playerNumber / 2];
	}

	void FixedUpdate () {
		if (animal != null) {
			animal.Move (xAxis().Value, -yAxis().Value); // y-axis is inverted by default

			if (dashButton().IsPressed) {
				animal.Dash();
			}
		}
	}

	private bool isOddNumbered () {
		return playerNumber % 2 == 1;
	}

	private InputControl xAxis () {
		return isOddNumbered() ? input.RightStickX : input.LeftStickX;
	}

	private InputControl yAxis () {
		return isOddNumbered() ? input.RightStickY : input.LeftStickY;
	}

	private InputControl dashButton () {
		return isOddNumbered() ? input.RightTrigger : input.LeftTrigger;
	}
}
