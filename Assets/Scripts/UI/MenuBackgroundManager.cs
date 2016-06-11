using UnityEngine;
using System.Collections;

public class MenuBackgroundManager : MonoBehaviour {
    public RectTransform readyUpBackground;
    public RectTransform characterChoiceBackground;
    public RectTransform boardChoiceBackground;

	public void ShowForReadyUp() {
        readyUpBackground.gameObject.SetActive(true);
        characterChoiceBackground.gameObject.SetActive(true);
        if (boardChoiceBackground) boardChoiceBackground.gameObject.SetActive(true); //TODO board choice
    }

    public void ShowForCharacterChoice() {
        readyUpBackground.gameObject.SetActive(false);
        characterChoiceBackground.gameObject.SetActive(true);
        if (boardChoiceBackground) boardChoiceBackground.gameObject.SetActive(true); //TODO board choice
    }

    public void ShowForBoardChoice() {
        readyUpBackground.gameObject.SetActive(false);
        characterChoiceBackground.gameObject.SetActive(false);
        if (boardChoiceBackground) boardChoiceBackground.gameObject.SetActive(true); //TODO board choice
    }

    public void HideAll() {
        readyUpBackground.gameObject.SetActive(false);
        characterChoiceBackground.gameObject.SetActive(false);
        if (boardChoiceBackground) boardChoiceBackground.gameObject.SetActive(false); //TODO board choice
    }
}
