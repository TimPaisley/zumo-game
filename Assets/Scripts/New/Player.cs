using Zumo.InputHelper;
using UnityEngine;

namespace Zumo {
	public class Player {
        static readonly Color DEFAULT_PLAYER_COLOR = Color.black;
        
        static readonly Color[] PLAYER_COLORS = {
			ColorHelper.fromHex("#2da6eb"),
            ColorHelper.fromHex("#7f70ef"),
            ColorHelper.fromHex("#e3732f"),
            ColorHelper.fromHex("#edad24"),
            ColorHelper.fromHex("#0dc2bc"),
            ColorHelper.fromHex("#8ccc12"),
            ColorHelper.fromHex("#e073d7"),
            ColorHelper.fromHex("#ef3655")
        };

		public Player(int index, InputMap input) {
			this.index = index;
			this.input = input;
		}

		public int index { get; private set; }

		public InputMap input { get; private set; }

        public int number {
            get { return index + 1; }
        }

		public string name {
			get { return "Player " + number; }
		}

		public string shortName {
			get { return "P" + number; }
		}

		public Color color {
			get { return index < PLAYER_COLORS.Length ? PLAYER_COLORS[index] : DEFAULT_PLAYER_COLOR; }
		}
	}
}
