using UnityEngine;
using InControl;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ReadyUpController : MonoBehaviour {
    public Sprite xboxSprite;
    public Text ltText;
    public Text rtText;

    public Image image;

    public PlayerController basePlayerController { get; set; } // Set by the parent ReadyUpMenu

    public PlayerController leftPlayer { get; private set; }
    public PlayerController rightPlayer { get; private set; }

	void Start () {
	}
	
	void Update () {
        if (ltText.gameObject.activeSelf && leftPlayer.isReady) {
            ltText.gameObject.SetActive(false);
        }
        if (rtText.gameObject.activeSelf && rightPlayer.isReady) {
            rtText.gameObject.SetActive(false);
        }
    }

    public void SetupPlayers (int leftPlayerIndex) {
        leftPlayer = Instantiate(basePlayerController);
        leftPlayer.Setup(leftPlayerIndex);
        rightPlayer = Instantiate(basePlayerController);
        rightPlayer.Setup(leftPlayerIndex + 1);

        if (isXboxController(leftPlayer.input.inputDevice)) {
            image.sprite = xboxSprite;
            ltText.text = "LT";
            rtText.text = "RT";
        }
    }

    private bool isXboxController(InputDevice device) {
        return device is UnityInputDevice && (device as UnityInputDevice).Profile.Name.Contains("XBox");
    }
}
