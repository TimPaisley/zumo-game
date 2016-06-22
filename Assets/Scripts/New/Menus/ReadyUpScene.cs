using System;
using System.Linq;
using UnityEngine;

namespace Zumo {
	class ReadyUpScene : VirtualScene {
        [Header("Ready Up Scene")]
		public VirtualScene nextScene;
		public ReadyUpDevice baseDeviceView;

		void Start() {
			Setup();

			createControllers();
		}

		void Update() {
		}

		public override void Load() {
			base.Load();
		}

		public override void Unload() {
			base.Unload();
		}

		private void createControllers() {
			foreach (var players in gameManager.players.GroupBy(player => player.input)) {
				var controllerView = Instantiate(baseDeviceView);
				controllerView.Setup(players);
			}
		}
	}
}
