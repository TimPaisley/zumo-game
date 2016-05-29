using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FollowAnimal : MonoBehaviour {
    public PlayerController player;
    public int yOffset = 70;

    private AnimalController animal;
    private RectTransform canvasRect;
    private RectTransform rectTransform;
    private Text text;
    private CameraManager cameraManager;

	// Use this for initialization
	void Start () {
        animal = player.animal;
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        cameraManager = FindObjectOfType<CameraManager>();

        var text = GetComponentInChildren<Text>();
        text.text = "P" + (player.playerIndex + 1);
        text.color = player.color;
	}
	
	// Update is called once per frame
	void Update () {
        var animalPos = cameraManager.mainCamera.WorldToViewportPoint(animal.transform.position);

        var screenPos = new Vector2(
            ((animalPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((animalPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f))
        );

        screenPos.y += yOffset + (yOffset / 30) * Mathf.Sin(Time.fixedTime * 5);

        rectTransform.anchoredPosition = screenPos;

        if (!player.isAlive) {
            gameObject.SetActive(false);
        }
    }
}
