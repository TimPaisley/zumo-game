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

    public InputDevice input { get; private set; }
    public bool leftPlayerReady { get; private set; }
    public bool rightPlayerReady { get; private set; }

	void Start () {
	}
	
	void Update () {
        if (input.LeftTrigger.IsPressed) {
            leftPlayerReady = true;
        } else if (input.RightTrigger.IsPressed) {
            rightPlayerReady = true;
        }
	}

    public void UseInput (InputDevice input) {
        this.input = input;

        if (input is UnityInputDevice && (input as UnityInputDevice).Profile.Name.Contains("XBox")) {
            image.sprite = xboxSprite;
            ltText.text = "LT";
            rtText.text = "RT";
        }
    }
}
