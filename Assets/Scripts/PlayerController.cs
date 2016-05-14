using UnityEngine;
using InControl;
using System.Collections;

public class PlayerController : MonoBehaviour {
    private class FakeInputControl : InputControl {
        public FakeInputControl () : base("", InputControlType.Action1) { }

        public new const float Value = 0;
        public new const bool IsPressed = false;
    }

    private class FakeInputDevice : InputDevice {
        public FakeInputDevice () : base("") { }

        public new InputControl RightStickX = new FakeInputControl();
        public new InputControl RightStickY = new FakeInputControl();
        public new InputControl LeftStickX = new FakeInputControl();
        public new InputControl LeftStickY = new FakeInputControl();
        public new InputControl RightTrigger = new FakeInputControl();
        public new InputControl LeftTrigger = new FakeInputControl();
    }

    // Global References


    // Player Variables
    public int playerNumber;
	public AnimalController animal;

	private InputDevice input;

	void Start () {
		input = (InputManager.Devices.Count > playerNumber / 2) ? InputManager.Devices[playerNumber / 2] : new FakeInputDevice();
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
