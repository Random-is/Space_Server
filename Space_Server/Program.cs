using System;
using Space_Server.game;
using Space_Server.game.ship_components;
using Space_Server.server;
using Space_Server.utility;

namespace Space_Server {
    internal static class Program {
        public static void Main(string[] args) {
            // var server = new Server(4444);
            // server.InitAndStart();
            // while (true) {
            //     Console.ReadLine();
            //     Log.Print("GC Collect -> Start");
            //     GC.Collect();
            //     Log.Print("GC Collect -> Complete");
            // }
            // const int length = 8;
            // var clients = new ConcurrentList<NetworkClient>();
            // for (var i = 0; i < length; i++) {
            //     clients.Add(new NetworkClient(new GamePlayer {Nickname = i.ToString()}));
            // }
            // var game = new Game(clients);
            // game.Generate();
            
            var front = new PersonArena();
            var back = new PersonArena();
            var pvp = new PvpArena();
            front.Arena[1, 1] = new SpaceShip(HullType.Diversant);
            front.Arena[0, 0] = new SpaceShip(HullType.Diversant);
            front.Arena[0, 4] = new SpaceShip(HullType.Diversant);
            pvp.LocateFrontPlayer(front);
            pvp.LocateBackPlayer(front);
            PrintMas(front.Arena);
            Console.WriteLine();
            PrintMas(pvp.Arena);
        }

        private static void PrintMas(SpaceShip[,] matrix) {
            var w = matrix.GetLength(0); // width
            var h = matrix.GetLength(1); // height
            for (var x = 0; x < w; x++) {
                for (var y = 0; y < h; y++) {
                    Console.Write($"{matrix[x, y]?.Hull} | ");
                }
                Console.WriteLine();
            }
        }
    }
}