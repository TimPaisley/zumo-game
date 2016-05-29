using UnityEngine;
using InControl;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ReadyUpController : MonoBehaviour {
    public Sprite xboxSprite;
    public Text leftPlayerText;
    public Text rightPlayerText;
    public RectTransform leftPlayerButton;
    public RectTransform rightPlayerButton;
    public RectTransform leftPlayerReadyIcon;
    public RectTransform rightPlayerReadyIcon;
    
    public Image image;

    public PlayerController basePlayerController { get; set; } // Set by the parent ReadyUpMenu

    public PlayerController leftPlayer { get; private set; }
    public PlayerController rightPlayer { get; private set; }
    
	void Update () {
        if (!leftPlayer.isReady && leftPlayer.input.dashButton.IsPressed) {
            leftPlayer.isReady = true;
            leftPlayerButton.gameObject.SetActive(false);
            leftPlayerReadyIcon.gameObject.SetActive(true);
        }
        
        if (!rightPlayer.isReady && rightPlayer.input.dashButton.IsPressed) {
            rightPlayer.isReady = true;
            rightPlayerButton.gameObject.SetActive(false);
            rightPlayerReadyIcon.gameObject.SetActive(true);
        }
    }

    public void SetupPlayers (int leftPlayerIndex) {
        leftPlayer = Instantiate(basePlayerController);
        leftPlayer.Setup(leftPlayerIndex);
        rightPlayer = Instantiate(basePlayerController);
        rightPlayer.Setup(leftPlayerIndex + 1);

        leftPlayerText.text = leftPlayer.playerName;
        leftPlayerText.color = leftPlayer.color;
        rightPlayerText.text = rightPlayer.playerName;
        rightPlayerText.color = rightPlayer.color;

        if (isXboxController(leftPlayer.input.inputDevice)) {
            image.sprite = xboxSprite;
            leftPlayerButton.GetComponentInChildren<Text>().text = "LT";
            rightPlayerButton.GetComponentInChildren<Text>().text = "RT";
        } else if (isKeyboardController(leftPlayer.input.inputDevice)) {
            // You can't disable a CanvasRenderer, so just set the scale to zero
            transform.localScale = Vector3.zero;
        }
    }

    private bool isXboxController(InputDevice device) {
        return device is UnityInputDevice && (device as UnityInputDevice).Profile.Name.Contains("XBox");
    }

    private bool isKeyboardController(InputDevice device) {
        return device is InputMapping.KeyboardInputDevice;
    }
}
