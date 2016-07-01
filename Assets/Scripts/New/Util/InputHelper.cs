using System.Collections.Generic;
using UnityEngine;
using InControl;
using System.Linq;

namespace Zumo.InputHelper {
	enum InputType {
		PSController,
		XboxController,
		Keyboard
	}

	interface InputStick {
		float xAxis { get; }

		float yAxis { get; }

		Vector2 position { get; }
	}

	interface InputButton {
		bool isPressed { get; }

		bool wasPressed { get; }

		bool wasReleased { get; }
	}

	interface InputMap {
		int deviceIndex { get; }

		InputStick joystick { get; }

		InputButton dash { get; }

		InputButton ability { get; }

		InputButton confirm { get; }

		InputButton back { get; }

		InputButton menu { get; }

		InputType inputType { get; }
	}

	class KeyboardAxes : InputStick {
		private string xAxisName;
		private string yAxisName;

		public KeyboardAxes(string xAxisName, string yAxisName) {
			this.xAxisName = xAxisName;
			this.yAxisName = yAxisName;
		}

		public float xAxis {
			get { return Input.GetAxis(xAxisName); }
		}

		public float yAxis {
			get { return Input.GetAxis(yAxisName); }
		}

		public Vector2 position {
			get { return new Vector2(xAxis, yAxis); }
		}
	}

	class KeyboardKey : InputButton {
		private KeyCode keyCode;

		public KeyboardKey(KeyCode keyCode) {
			this.keyCode = keyCode;
		}

		public bool isPressed {
			get { return Input.GetKey(keyCode); }
		}

		public bool wasPressed {
			get { return Input.GetKeyDown(keyCode); }
		}

		public bool wasReleased {
			get { return Input.GetKeyUp(keyCode); }
		}
	}

	class InControlJoystick : InputStick {
		private InputControl xAxisControl;
		private InputControl yAxisControl;

		public InControlJoystick(InputControl xAxisControl, InputControl yAxisControl) {
			this.xAxisControl = xAxisControl;
			this.yAxisControl = yAxisControl;
		}

		public float xAxis {
			get { return xAxisControl.Value; }
		}

		public float yAxis {
			get { return yAxisControl.Value; }
		}

		public Vector2 position {
			get { return new Vector2(xAxis, yAxis); }
		}
	}

	class InControlButton : InputButton {
		private InputControl control;

		public InControlButton(InputControl control) {
			this.control = control;
		}

		public bool isPressed {
			get { return control.IsPressed; }
		}

		public bool wasPressed {
			get { return control.WasPressed; }
		}

		public bool wasReleased {
			get { return control.WasReleased; }
		}
	}

	enum InputSide {
		Left,
		Right

	}

	class KeyboardInput : InputMap {
		public KeyboardInput(InputSide side) {
			deviceIndex = InputManager.Devices.Count;

			joystick = side == InputSide.Left ?
                new KeyboardAxes("Horizontal", "Vertical") :
                new KeyboardAxes("Horizontal 2", "Vertical 2");

			dash = side == InputSide.Left ?
                new KeyboardKey(KeyCode.Space) :
                new KeyboardKey(KeyCode.RightShift);
			ability = side == InputSide.Left ?
                new KeyboardKey(KeyCode.Q) :
                new KeyboardKey(KeyCode.Slash);

			confirm = new KeyboardKey(KeyCode.Return);
			back = new KeyboardKey(KeyCode.Backspace);
			menu = new KeyboardKey(KeyCode.Escape);
		}

		public int deviceIndex { get; private set; }

		public InputStick joystick { get; private set; }

		public InputButton dash { get; private set; }

		public InputButton ability { get; private set; }

		public InputButton confirm { get; private set; }

		public InputButton back { get; private set; }

		public InputButton menu { get; private set; }

		public InputType inputType { get { return InputType.Keyboard; } }
	}

	class FindKeyboardInputs {
		public static KeyboardInput[] call () {
			return new KeyboardInput[] {
				new KeyboardInput(InputSide.Left),
				new KeyboardInput(InputSide.Right)
			};
		}
	}

	class ControllerInput : InputMap {
		public ControllerInput(int index, InputSide side) {
			var device = InputManager.Devices[index];

			deviceIndex = index;

			joystick = side == InputSide.Left ? 
                new InControlJoystick(device.LeftStickX, device.LeftStickY) :
                new InControlJoystick(device.RightStickX, device.RightStickY);

			dash = side == InputSide.Left ?
                new InControlButton(device.LeftTrigger) :
                new InControlButton(device.RightTrigger);
			ability = side == InputSide.Left ?
                new InControlButton(device.LeftStickButton) :
                new InControlButton(device.RightStickButton);

			confirm = new InControlButton(device.Action1);
			back = new InControlButton(device.Action2);
			menu = new InControlButton(device.Action4);

			inputType = isXboxController(device) ? InputType.XboxController : InputType.PSController;
		}

		public int deviceIndex { get; private set; }

		public InputStick joystick { get; private set; }

		public InputButton dash { get; private set; }

		public InputButton ability { get; private set; }

		public InputButton confirm { get; private set; }

		public InputButton back { get; private set; }

		public InputButton menu { get; private set; }

		public InputType inputType { get; private set; }

		private bool isXboxController (InputDevice device) {
			return device is UnityInputDevice && (device as UnityInputDevice).Profile.Name.Contains("XBox");
		}
	}

	class FindControllerInputs {
		public static IEnumerable<ControllerInput> call () {
			var instances = new List<ControllerInput>(InputManager.Devices.Count * 2);

			foreach (var index in Enumerable.Range(0, InputManager.Devices.Count)) {
				instances.Add(new ControllerInput(index, InputSide.Left));
				instances.Add(new ControllerInput(index, InputSide.Right));
			}

			return instances;
		}
	}
}
