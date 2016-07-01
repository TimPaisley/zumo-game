using Zumo.InputHelper;
using UnityEngine;
using System.Linq;

namespace Zumo {
	class PlayerController {
		private readonly Color[] PLAYER_COLORS = new Color[] {
			ColorHelper.fromHex("#FFFFFF")
		};

		public PlayerController(int index, InputMap input) {
			this.index = index;
			this.input = input;

			isReady = false;
		}

		public bool isReady { get; set; }

		public AnimalController chosenAnimal { get; set; }

		public int index { get; private set; }

		public InputMap input { get; private set; }

		public string name {
			get { return "Player " + index; }
		}

		public string shortName {
			get { return "P" + index; }
		}

		public Color color {
			get { return index < PLAYER_COLORS.Length ? PLAYER_COLORS[index] : Color.black; }
		}
	}
}
