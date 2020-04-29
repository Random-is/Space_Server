using System;
using System.Net.Sockets;
using Space_Server.controller;
using Space_Server.model;

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
            // var clients = new ConcurrentList<NetworkClient>();
            // for (var i = 0; i < 8; i++) {
            //     clients.Add(new NetworkClient(new TcpClient(), new GamePlayer()));
            // }
            // var game = new Game(clients);
            // game.Generate();
        }
    }
}