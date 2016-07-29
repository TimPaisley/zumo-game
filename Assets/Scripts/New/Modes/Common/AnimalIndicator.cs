using UnityEngine;
using UnityEngine.UI;

namespace Zumo {
	public class AnimalIndicator : MonoBehaviour {
        const int Y_OFFSET = 70;

        Animal animal;
        string playerName;

        RectTransform canvasRect;
        RectTransform rectTransform;
        CanvasRenderer canvasRenderer;
        CameraManager cameraManager;

        void Start () {
            canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            rectTransform = GetComponent<RectTransform>();
            canvasRenderer = GetComponent<CanvasRenderer>();
            cameraManager = FindObjectOfType<CameraManager>();
        }

        void Update () {
            var animalPos = cameraManager.currentCamera.WorldToViewportPoint(animal.transform.position);

            var screenPos = new Vector2(
                ((animalPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
                ((animalPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f))
            );

            screenPos.y += Y_OFFSET + (Y_OFFSET / 30) * Mathf.Sin(Time.fixedTime * 5);

            rectTransform.anchoredPosition = screenPos;
        }

        public void Setup (Animal animal) {
            this.animal = animal;

            var text = GetComponentInChildren<Text>();
            text.text = animal.player.shortName;
            text.color = animal.player.color;
        }
    }
}
