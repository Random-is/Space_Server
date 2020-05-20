using System;
using Space_Server.game;
using Space_Server.server;
using Space_Server.utility;

namespace Space_Server {
    internal static class Program {
        public static void Main(string[] args) {
            var server = new Server(4444);
            server.InitAndStart();
            while (true) {
                Console.ReadLine();
                Log.Print("GC Collect -> Start");
                GC.Collect();
                Log.Print("GC Collect -> Complete");
            }
            // const int length = 8;
            // var clients = new ConcurrentList<NetworkClient>();
            // for (var i = 0; i < length; i++) {
            //     clients.Add(new NetworkClient(new GamePlayer {Nickname = i.ToString()}));
            // }
            // var game = new Game(clients);
            // game.Generate();
        }
    }
}