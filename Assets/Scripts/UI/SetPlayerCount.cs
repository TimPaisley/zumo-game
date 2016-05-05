using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SetPlayerCount : MonoBehaviour {
	public int minValue = 2;
	public int maxValue = 4;

	public int value { get; private set; }

	private Text text;

	void Start() {
		value = minValue;
		text = GetComponentInChildren<Text>();
		text.text = value.ToString();
	}

	public void OnInput(float axisPosition) {
		if (axisPosition > 0 && value < maxValue) {
			value++;
		} else if (axisPosition < 0 && value > minValue) {
			value--;
		}
		text.text = value.ToString();
	}
}
