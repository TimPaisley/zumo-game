using System.Collections.Generic;
using System.Linq;

namespace Zumo {
	class GameState {
		public Player[] players { get; private set; }
		public Board chosenBoard;

		Dictionary<Player, bool> playerReadyStates;
		Dictionary<Player, Animal> playerAnimals;

		public GameState (Player[] allPlayers) {
			players = allPlayers;
			playerAnimals = new Dictionary<Player, Animal>();
			playerReadyStates = new Dictionary<Player, bool>();
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

			chosenBoard = null;
		}
	}
}
