using System.Collections.Generic;
using System.Threading;
using Space_Server.controller;

namespace Space_Server.model {
    public class Room {
        public ConcurrentList<NetworkClient> Clients { get; }
        public ConcurrentList<GamePlayer> GamePlayers { get; } = new ConcurrentList<GamePlayer>();
        public Game Game { get; set; }

        public Room(ConcurrentList<NetworkClient> clients) {
            Clients = clients;
        }

        public void Start() {
            var gameThread = new Thread(CreateGame);
            gameThread.Start();
        }

        private void CreateGame() {
            Game = new Game(Clients);
            Game.Generate();
            Net.WaitAll(Clients, "ROOM_LOADED", out var handled, () => Net.SendAll(Clients, "ROOM_CREATED"));
            Game.Start();
        }
    }
}