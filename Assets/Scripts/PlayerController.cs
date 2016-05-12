using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	// Global References


	// Player Variables
	public int playerNumber;
	public AnimalController animal;

	void Start () {
		
	}

	void FixedUpdate () {
		if (animal != null) {
			float h = Input.GetAxisRaw ("Horizontal " + playerNumber);
			float v = Input.GetAxisRaw ("Vertical " + playerNumber);

			animal.Move (-h, v); // not entirely sure why h needs to be negative here...
		}
	}
}
