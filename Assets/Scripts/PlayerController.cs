using UnityEngine;
using InControl;
using System.Collections;

public class PlayerController : MonoBehaviour {
    

    // Global References


    // Player Variables
    public int playerIndex;
	public AnimalController animal;
    public Renderer board;

    public bool isAlive { get; private set; }

	private InputMapping input;

	void Start () {
		InputMapping.Side controllerSide = playerIndex % 2 == 1 ? InputMapping.Side.RIGHT : InputMapping.Side.LEFT;

		input = new InputMapping(playerIndex / 2, controllerSide);
        isAlive = true;
	}

	void FixedUpdate () {
		if (isAlive) {
			animal.Move (input.xAxis.Value, -input.yAxis.Value); // y-axis is inverted by default

			if (input.dashButton.IsPressed) {
				animal.Dash();
			}

            if (!animal.isInBounds) {
                isAlive = false;
                animal.Kill();
            }
		}
	}
}
