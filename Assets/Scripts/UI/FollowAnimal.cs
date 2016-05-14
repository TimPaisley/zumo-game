using UnityEngine;
using System.Collections;

public class FollowAnimal : MonoBehaviour {
    public Transform animal;
    public Camera camera;
    public int yOffset = 100;

    private RectTransform canvasRect;
    private RectTransform rectTransform;

	// Use this for initialization
	void Start () {
        canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        var animalPos = camera.WorldToViewportPoint(animal.position);

        var screenPos = new Vector2(
            ((animalPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
            ((animalPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f))
        );

        screenPos.y += yOffset;

        rectTransform.anchoredPosition = screenPos;
    }
}
