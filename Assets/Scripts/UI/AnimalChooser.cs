using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class AnimalChooser : MonoBehaviour {
    [Header("Should not be set in the editor")]
    public PlayerController player;
    public bool hasChosen = false;

    private AxisInput xAxisInput;
    private AxisInput yAxisInput;

    private Image borderImage;
    private Text playerText;

    public void MatchPlayer (PlayerController player) {
        borderImage = GetComponentsInChildren<Image>().Last();
        playerText = GetComponentInChildren<Text>();

        playerText.text = player.shortName;
        playerText.color = player.color;
        borderImage.color = player.color;

        this.player = player;
        xAxisInput = new AxisInput(player.input.xAxis);
        yAxisInput = new AxisInput(player.input.yAxis);
    }

    public Vector2 DesiredMoveLocation (Vector2 currentLocation) {
        var xMove = xAxisInput.CheckInput();
        var yMove = yAxisInput.CheckInput();

        // Special case where moving from NoPosition to a position
        if (currentLocation == CharacterChoiceScene.NoPosition && yMove < 0) {
            return new Vector2(0, 0);
        }

        var newLocation = new Vector2(currentLocation.x, currentLocation.y);

        if (xMove < 0) {
            newLocation.x -= 1;
        } else if (xMove > 0) {
            newLocation.x += 1;
        }

        // Handle inverted y-axis
        if (yMove > 0) {
            newLocation.y -= 1;
        } else if (yMove < 0) {
            newLocation.y += 1;
        }

        return newLocation;
    }
}
