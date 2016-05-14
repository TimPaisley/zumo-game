using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FollowAnimal : MonoBehaviour {
    public PlayerController player;
    public Camera worldCamera;
    public int yOffset = 70;

    private Transform animal;
    private RectTransform canvasRect;
    private RectTransform rectTransform;
    private Text text;

	// Use this for initialization
	void Start () {
        animal = player.animal.transform;
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();

        GetComponentInChildren<Text>().text = "P" + (player.playerIndex + 1);
	}
	
	// Update is called once per frame
	void Update () {
        var animalPos = worldCamera.WorldToViewportPoint(animal.position);

        var screenPos = new Vector2(
            ((animalPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((animalPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f))
        );

        screenPos.y += yOffset + (yOffset / 30) * Mathf.Sin(Time.fixedTime * 5);

        rectTransform.anchoredPosition = screenPos;
    }
}
