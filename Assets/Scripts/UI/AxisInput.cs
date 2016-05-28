using UnityEngine;
using UnityEngine.Events;
using InControl;
using System.Collections;

public class AxisInput {
	private InputControl axis;
    private float sensitivity;

	private bool awaitingInput = true;
	private float lastPosition = 0;

    public AxisInput(InputControl axis, float sensitivity = 0.2f) {
        this.axis = axis;
        this.sensitivity = sensitivity;
    }

    public float CheckInput () {
        float moveValue = 0;

        if (awaitingInput) {
            moveValue = checkPositiveMovement(axis.Value);
        } else {
            checkReverseMovement(axis.Value);
        }

        lastPosition = axis.Value;

        return moveValue;
    }

	private float checkPositiveMovement(float position) {
		if (Mathf.Abs(position) > Mathf.Abs(lastPosition) && Mathf.Abs(position) > sensitivity) {
			awaitingInput = false;
        
            return position;
		}
        return 0;
	}

	private void checkReverseMovement(float position) {
		if (Mathf.Abs(position) < Mathf.Abs(lastPosition)) {
			awaitingInput = true;
		}
	}
}
