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

	private class KeyInputControl : InputControl {
		private KeyCode key;

		public KeyInputControl (KeyCode key) : base("", InputControlType.Analog0) {
			this.key = key;
		}

		public override bool IsPressed {
			get { return Input.GetKey(key); }
		}

		public override string ToString() {
			return string.Format("[KeyInputControl: IsPressed={0}]", IsPressed);
		}
	}

	private class KeyboardInputDevice : InputDevice {
		private InputControl leftStickX = new AxisInputControl("Horizontal");
		private InputControl leftStickY = new AxisInputControl("Vertical");
		private InputControl rightStickX = new AxisInputControl("Horizontal 2");
		private InputControl rightStickY = new AxisInputControl("Vertical 2");
		private InputControl leftTrigger = new KeyInputControl(KeyCode.Space);
		private InputControl rightTrigger = new KeyInputControl(KeyCode.RightShift);
		private InputControl action1 = new KeyInputControl(KeyCode.Return);

		public override InputControl LeftStickX {
			get { return leftStickX; }
		}
		public override InputControl LeftStickY {
			get { return leftStickY; }
		}
		public override InputControl RightStickX {
			get { return rightStickX; }
		}
		public override InputControl RightStickY {
			get { return rightStickY; }
		}
		public override InputControl RightTrigger {
			get { return rightTrigger; }
		}
		public override InputControl LeftTrigger {
			get { return leftTrigger; }
		}
		public override InputControl Action1 {
			get { return action1; }
		}

		public KeyboardInputDevice () : base("") { }
	}

	//TODO remove when there's a proper menu
	private class FakeInputDevice : InputDevice {
		public FakeInputDevice () : base("") { }
	}

	public InputDevice inputDevice { get; private set; }
	public InputControl xAxis { get; private set; }
	public InputControl yAxis { get; private set; }
	public InputControl dashButton { get; private set; }
	public InputControl actionButton { get; private set; }

	public InputMapping(int deviceIndex, Side side) {
		if (deviceIndex < InputManager.Devices.Count) {
			inputDevice = InputManager.Devices[deviceIndex];
		} else if (deviceIndex == InputManager.Devices.Count) {
			inputDevice = new KeyboardInputDevice();
		} else {
			inputDevice = new FakeInputDevice();
		}

		xAxis = side == Side.LEFT ? inputDevice.LeftStickX : inputDevice.RightStickX;
		yAxis = side == Side.LEFT ? inputDevice.LeftStickY : inputDevice.RightStickY;
		dashButton = side == Side.LEFT ? inputDevice.LeftTrigger : inputDevice.RightTrigger;
		actionButton = inputDevice.Action1;
	}
}
