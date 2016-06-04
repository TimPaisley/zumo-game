using UnityEngine;
using System.Collections;

public class MenuBackgroundManager : MonoBehaviour {
    public RectTransform readyUpBackground;
    public RectTransform characterChoiceBackground;

	public void ShowForReadyUp() {
        readyUpBackground.gameObject.SetActive(true);
        characterChoiceBackground.gameObject.SetActive(true);
    }

    public void ShowForCharacterChoice() {
        readyUpBackground.gameObject.SetActive(false);
        characterChoiceBackground.gameObject.SetActive(true);
    }

    public void HideAll() {
        readyUpBackground.gameObject.SetActive(false);
        characterChoiceBackground.gameObject.SetActive(false);
    }
}
