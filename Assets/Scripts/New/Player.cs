using Zumo.InputHelper;
using UnityEngine;

namespace Zumo {
	public class Player {
		static readonly Color[] PLAYER_COLORS = {
			ColorHelper.fromHex("#FFFFFF")
		};

		public Player(int index, InputMap input) {
			this.index = index;
			this.input = input;
		}

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
