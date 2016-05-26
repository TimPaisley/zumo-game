using UnityEngine;
using InControl;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class ReadyUpMenu : MonoBehaviour {
    private int MIN_PLAYERS = 2;

    public ReadyUpController baseControllerView;
    public GameObject startIndicator;

    private RectTransform rectTransform;
    private List<ReadyUpController> controllerViews;

	void Start () {
        rectTransform = GetComponent<RectTransform>();
        controllerViews = new List<ReadyUpController>(InputManager.Devices.Count + 1);

        startIndicator.SetActive(false);

        var yOffset = -9.5f;

	    foreach (var device in InputManager.Devices) {
            controllerViews.Add(createControllerView(device, yOffset));

            yOffset -= 8;
        }

        baseControllerView.gameObject.SetActive(false);
	}
	
	void Update () {
	    if (!startIndicator.activeSelf && readyPlayers() >= MIN_PLAYERS) {
            startIndicator.SetActive(true);
        }
	}

    private ReadyUpController createControllerView (InputDevice device, float yOffset) {
        var controllerView = Instantiate(baseControllerView.gameObject);
        var viewTransform = controllerView.GetComponent<RectTransform>();
        var controllerComponent = controllerView.GetComponent<ReadyUpController>();

        viewTransform.parent = rectTransform;
        viewTransform.position = Vector3.zero;
        viewTransform.localEulerAngles = Vector3.zero;
        viewTransform.anchoredPosition = new Vector2(0, yOffset);

        controllerComponent.UseInput(device);

        return controllerComponent;
    }

    private int readyPlayers() {
        int count = 0;

        foreach (var controller in controllerViews) {
            if (controller.leftPlayerReady) {
                count++;
            }
            if (controller.rightPlayerReady) {
                count++;
            }
        }

        return count;
    }
}
