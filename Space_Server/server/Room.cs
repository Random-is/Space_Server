using System.Threading;
using Space_Server.game;
using Space_Server.utility;

namespace Space_Server.server {
    public class Room {
        public ConcurrentList<NetworkClient> Clients { get; }
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