using System;
using System.Linq;

namespace Zumo {
	class ReadyUpScene : VirtualScene {
		public VirtualScene nextScene;
		public ReadyUpController baseControllerView;

		void Start() {
			SetupReferences();

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
				var controllerView = Instantiate(baseControllerView);
				controllerView.Setup(players);
			}
		}
	}
}
