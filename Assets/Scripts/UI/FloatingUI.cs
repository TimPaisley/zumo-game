using UnityEngine;
using System.Collections;

public class FloatingUI : MonoBehaviour {
    public float maxOffset = 1;

    private RectTransform rectTransform;
    private float originalY;

	void Start () {
        rectTransform = GetComponent<RectTransform>();
        originalY = rectTransform.anchoredPosition.y;
	}
	
	// Update is called once per frame
	void Update () {
        rectTransform.anchoredPosition = new Vector2(
            rectTransform.anchoredPosition.x,
            originalY + Mathf.Sin(Time.fixedTime * 5) * maxOffset
        );
	}
}
