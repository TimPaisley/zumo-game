using UnityEngine;
using InControl;
using System.Collections;

public class InputMapping {
	public enum Side { LEFT, RIGHT };

	private class AxisInputControl : InputControl {
		private string axisName;

		public AxisInputControl (string axisName) : base("", InputControlType.Analog0) {
			this.axisName = axisName;
		}

		public override float Value {
			get { return Input.GetAxis(axisName); }
		}

		public override string ToString() {
			return string.Format("[AxisInputControl: Value={0}]", Value);
		}
	}

	public class KeyInputControl : InputControl {
		private KeyCode key;

		public KeyInputControl (KeyCode key) : base("", InputControlType.Analog0) {
			this.key = key;
		}

		public override bool IsPressed {
			get { return Input.GetKey(key); }
		}

		public override bool WasPressed {
			get { return Input.GetKeyDown(key); }
		}

		public override bool WasReleased {
			get { return Input.GetKeyUp(key); }
		}

		public override string ToString() {
			return string.Format("[KeyInputControl: IsPressed={0}]", IsPressed);
		}
	}

	public class FakeInputDevice : InputDevice {
		public FakeInputDevice () : base("") { }
	}

	public InputDevice inputDevice { get; private set; }
	public InputControl xAxis { get; private set; }
	public InputControl yAxis { get; private set; }
	public InputControl dashButton { get; private set; }
	public InputControl abilityButton { get; private set; }
	public InputControl actionButton { get; private set; }
	public InputControl backButton { get; private set; }
	public InputControl menuButton { get; private set; }

	public InputMapping(int deviceIndex, Side side) {
		if (deviceIndex < InputManager.Devices.Count) {
			inputDevice = InputManager.Devices[deviceIndex];
			setupControllerControls(side);
		} else {
			inputDevice = new FakeInputDevice();
			setupKeyboardControls(side);
		}
	}

	public void setupKeyboardControls(Side side) {
		xAxis = side == Side.LEFT ? new AxisInputControl ("Horizontal") : new AxisInputControl ("Horizontal 2");
		yAxis = side == Side.LEFT ? new AxisInputControl ("Vertical") : new AxisInputControl ("Vertical 2");
		dashButton = side == Side.LEFT ? new KeyInputControl (KeyCode.Space) : new KeyInputControl (KeyCode.RightShift);
		abilityButton = side == Side.LEFT ? new KeyInputControl (KeyCode.Z) : new KeyInputControl (KeyCode.Slash);

		actionButton = new KeyInputControl (KeyCode.Return);
		backButton = new KeyInputControl (KeyCode.Backspace);
		menuButton = new KeyInputControl (KeyCode.Escape);
	}

	public void setupControllerControls(Side side) {
		xAxis = side == Side.LEFT ? inputDevice.LeftStickX : inputDevice.RightStickX;
		yAxis = side == Side.LEFT ? inputDevice.LeftStickY : inputDevice.RightStickY;
		dashButton = side == Side.LEFT ? inputDevice.LeftTrigger : inputDevice.RightTrigger;
		abilityButton = side == Side.LEFT ? inputDevice.DPadUp : inputDevice.Action4;

		actionButton = inputDevice.Action1;
		backButton = inputDevice.Action2;
		menuButton = inputDevice.Action4;
	}
}
