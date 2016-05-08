using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[System.Serializable]
public class AxisMovedEvent : UnityEvent<float> { }

public class AxisInput : MonoBehaviour {
	public string axis;
	public AxisMovedEvent onMove;

	private bool awaitingInput = true;
	private float lastPosition = 0;

	void Start() {
		if (onMove == null) onMove = new AxisMovedEvent();
	}

	void Update() {
		float currentPosition = Input.GetAxis(axis);

		if (awaitingInput) {
			checkInput(currentPosition);
		} else {
			checkReverseMovement(currentPosition);
		}

		lastPosition = currentPosition;
	}

	private void checkInput(float position) {
		if (Mathf.Abs(position) > Mathf.Abs(lastPosition)) {
			awaitingInput = false;
			onMove.Invoke(position);
		}
	}

	private void checkReverseMovement(float position) {
		if (Mathf.Abs(position) < Mathf.Abs(lastPosition)) {
			awaitingInput = true;
		}
	}
}
