using System.Collections.Generic;
using Space_Server.controller;

namespace Space_Server.model {
    public class Room {
        public List<NetworkClient> Players { get; }
        
        public List<GamePlayer> GamePlayers { get; } = new List<GamePlayer>();

        public Room(List<NetworkClient> players) {
            Players = players;
        }

        public void Start() {
            Net.SendAll(Players, "GAME CREATED");
            Players.ForEach(player => GamePlayers.Add(player.Player));
            foreach (var player in Players) {
                player.Player.Set(100, 0, 0, 1, 
                    new List<SpaceShip>(), new List<Component>());
                GamePlayers.Add(player.Player);
            }
        }
    }
}