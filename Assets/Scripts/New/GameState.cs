using System.Collections.Generic;
using System.Linq;

namespace Zumo {
	class GameState {
		public Player[] players { get; private set; }
		public Board chosenBoard { get; set; }

		Dictionary<Player, bool> playerReadyStates = new Dictionary<Player, bool>();
		Dictionary<Player, Animal> playerAnimals = new Dictionary<Player, Animal>();

		public GameState (Player[] allPlayers) {
			players = allPlayers;

			Reset();
		}

		public IEnumerable<Player> readyPlayers {
			get { return players.Where(player => playerReadyStates[player]); }
		}

		public void ReadyUp (Player player) {
			playerReadyStates[player] = true;
		}

		public void ChooseAnimal (Player player, Animal animal) {
			playerAnimals[player] = animal;
		}

		public void Reset () {
			playerReadyStates.Clear();
			playerAnimals.Clear();

			foreach (var player in players) {
				playerAnimals.Add(player, null);
				playerReadyStates.Add(player, false);
			}

			chosenBoard = null;
		}
	}
}
