using UnityEngine;
using System.Collections;

public class CharacterChooser : MonoBehaviour {
	public GameObject[] characters;
	public int charactersPerRow = 2; //TODO not used
	public int xOffset = 0;

	public GameObject selectedCharacter {
		get { return characters[currentIndex]; }
	}

	private int currentIndex = 0;

	void Start() {
		currentIndex = 0;
		updateIndicatorPosition();
	}

	public void OnHorizontalInput(float axisPosition) {
		if (axisPosition > 0 && currentIndex < characters.Length - 1) {
			currentIndex++;
			updateIndicatorPosition();
		} else if (axisPosition < 0 && currentIndex > 0) {
			currentIndex--;
			updateIndicatorPosition();
		}
	}

	private void updateIndicatorPosition() {
		var targetPosition = selectedCharacter.transform.localPosition;
		Debug.Log(targetPosition);
		targetPosition.y = 10; // Make sure the indicator is above the character
		targetPosition.x += xOffset;

		transform.localPosition = targetPosition;
	}
}
