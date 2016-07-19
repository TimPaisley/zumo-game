using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zumo.InputHelper;

namespace Zumo {
	public class ReadyUp : MonoBehaviour {
        public Camera sceneCamera;

        [Header("Device Views")]
		public ReadyUpDevice psControllerView;
        public ReadyUpDevice xboxControllerView;
        public ReadyUpDevice keyboardView;
        public float baseY = -9.5f;
        public float yIncrement = -8f;

        GameManager gm;
        List<ReadyUpDevice> deviceViews = new List<ReadyUpDevice>();

        void Awake() {
            gm = FindObjectOfType<GameManager>();
            psControllerView.gameObject.SetActive(false);
            xboxControllerView.gameObject.SetActive(false);
            keyboardView.gameObject.SetActive(false);
            sceneCamera.gameObject.SetActive(false);
        }

		void Start() {
            gm.cameraManager.Use(sceneCamera);
            gm.musicManager.Play(gm.musicManager.menuSong);

			createDeviceViews();
		}

		void Update() {
            if (deviceViews.All(view => view.bothPlayersReady) &&
                    gm.state.readyPlayers.Any(player => player.input.confirm.isPressed)) {
                gm.SwitchScene(gm.animalChoiceScene);
            }
		}

		private void createDeviceViews() {
            var yOffset = baseY;

			foreach (var players in gm.state.players.GroupBy(player => player.input.deviceIndex)) {
                var deviceView = createDeviceView(players.First().input, yOffset);

				deviceView.Setup(players);
                deviceViews.Add(deviceView);

                yOffset += yIncrement;
			}
		}

        private ReadyUpDevice createDeviceView(InputMap input, float yOffset) {
            ReadyUpDevice baseDeviceView;

            if (input.inputType == InputType.Keyboard) {
                baseDeviceView = keyboardView;
            } else if (input.inputType == InputType.XboxController) {
                baseDeviceView = xboxControllerView;
            } else {
                baseDeviceView = psControllerView;
            }

            var deviceView = Instantiate(baseDeviceView);
            var viewTransform = deviceView.GetComponent<RectTransform>();
            
            viewTransform.SetParent(baseDeviceView.transform.parent, false);
            viewTransform.localEulerAngles = Vector3.zero;
            viewTransform.anchoredPosition = new Vector2(0, yOffset);

            deviceView.gameObject.SetActive(true);

            return deviceView;
        }
    }
}
