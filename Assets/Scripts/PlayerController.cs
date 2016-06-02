using UnityEngine;
using InControl;
using System.Collections;

public class PlayerController : MonoBehaviour {
    private static Color rgb(int r, int g, int b) {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    public static readonly Color[] PlayerColors = {
        rgb(243, 21, 81),
        rgb(36, 154, 192),
        rgb(255, 149, 35),
        rgb(177, 96, 255),
		Color.black,
		Color.black
    };

    // Player Variables
    public AnimalController animal;
    public Renderer board;
    public int playerIndex;

    public bool isAlive { get; set; }
    public bool isReady { get; set; }
    public InputMapping input { get; private set; }

    public string playerName {
        get { return "Player " + (playerIndex + 1); }
    }

    public string shortName {
        get { return "P" + (playerIndex + 1); }
    }

    public Color color {
        get { return PlayerColors[playerIndex]; }
    }

    void Awake () {
        isReady = false;
        isAlive = false;
    }

	void FixedUpdate () {
		if (isReady && isAlive) {
			if(animal.dashIsCharging){
				animal.Rotate (input.xAxis.Value, -input.yAxis.Value); // y-axis is inverted by default
			}
			else{
				animal.Move (input.xAxis.Value, -input.yAxis.Value); // y-axis is inverted by default
			}
			if (input.dashButton.IsPressed) {
				animal.dashCharge();
			}
			if(input.dashButton.WasReleased){
				animal.Dash();
			}

			if (input.dashButton.WasReleased) {

			}

            if (!animal.isInBounds) {
                isAlive = false;
                animal.Kill();
            }
		}
	}

    public void Setup (int index) {
        playerIndex = index;
        InputMapping.Side controllerSide = playerIndex % 2 == 1 ? InputMapping.Side.RIGHT : InputMapping.Side.LEFT;

        input = new InputMapping(playerIndex / 2, controllerSide);
    }
}