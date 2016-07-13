using System.Collections.Generic;

namespace Zumo {
	public class GameState {
		public readonly Player[] players;
		public readonly HashSet<Player> readyPlayers = new HashSet<Player>();
		public readonly Dictionary<Player, Animal> chosenAnimals = new Dictionary<Player, Animal>();
		public Board chosenBoard;

		public GameState (Player[] allPlayers) {
			players = allPlayers;

			Reset();
		}

		public void ChooseAnimal (Player player, Animal animal) {
			chosenAnimals[player] = animal;
		}

		public void Reset () {
			readyPlayers.Clear();
			chosenAnimals.Clear();

			foreach (var player in players) {
				chosenAnimals.Add(player, null);
			}

			chosenBoard = null;
		}
	}
}
