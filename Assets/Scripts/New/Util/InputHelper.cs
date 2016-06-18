using System.Collections.Generic;
using UnityEngine;
using InControl;

namespace Zumo.InputHelper {
    interface InputStick {
        float xAxis { get; }
        float yAxis { get; }
    }

    interface InputButton {
        bool isPressed { get; }
        bool wasPressed { get; }
        bool wasReleased { get; }
    }

    interface InputMap {
        InputStick joystick { get; }

        InputButton dash { get; }
        InputButton ability { get; }

        InputButton confirm { get; }
        InputButton back { get; }
        InputButton menu { get; }
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

    enum InputSide { Left, Right }

    class KeyboardInput : InputMap {
        public static IEnumerable<KeyboardInput> instances { get; private set; }

        static KeyboardInput() {
            instances = new KeyboardInput[] {
                new KeyboardInput(InputSide.Left),
                new KeyboardInput(InputSide.Right)
            };
        }

        private KeyboardInput(InputSide side) {
            joystick = side == InputSide.Left ?
                new KeyboardAxes("Horizontal", "Vertical") :
                new KeyboardAxes("Horizontal 2", "Vertical 2");

            dash = side == InputSide.Left ?
                new KeyboardKey(KeyCode.LeftShift) :
                new KeyboardKey(KeyCode.RightShift);
            ability = side == InputSide.Left ?
                new KeyboardKey(KeyCode.Q) :
                new KeyboardKey(KeyCode.Slash);

            confirm = new KeyboardKey(KeyCode.Return);
            back = new KeyboardKey(KeyCode.Backspace);
            menu = new KeyboardKey(KeyCode.Escape);
        }

        public InputStick joystick { get; private set; }

        public InputButton dash { get; private set; }
        public InputButton ability { get; private set; }

        public InputButton confirm { get; private set; }
        public InputButton back { get; private set; }
        public InputButton menu { get; private set; }
    }

    class ControllerInput : InputMap {
        public static IEnumerable<ControllerInput> instances { get; private set; }

        static ControllerInput() {
            var allInstances = new List<ControllerInput>(InputManager.Devices.Count * 2);

            foreach (var device in InputManager.Devices) {
                allInstances.Add(new ControllerInput(device, InputSide.Left));
                allInstances.Add(new ControllerInput(device, InputSide.Right));
            }

            instances = allInstances;
        }

        private ControllerInput(InputDevice device, InputSide side) {
            joystick = side == InputSide.Left ? 
                new InControlJoystick(device.LeftStickX, device.LeftStickY) :
                new InControlJoystick(device.RightStickX, device.RightStickY);

            dash = side == InputSide.Left ?
                new InControlButton(device.LeftBumper) :
                new InControlButton(device.RightBumper);
            ability = side == InputSide.Left ?
                new InControlButton(device.LeftStickButton) :
                new InControlButton(device.RightStickButton);

            confirm = new InControlButton(device.Action1);
            back = new InControlButton(device.Action2);
            menu = new InControlButton(device.Action4);
        }

        public InputStick joystick { get; private set; }

        public InputButton dash { get; private set; }
        public InputButton ability { get; private set; }

        public InputButton confirm { get; private set; }
        public InputButton back { get; private set; }
        public InputButton menu { get; private set; }
    }
}
