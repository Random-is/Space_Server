using System;
using System.Collections.Generic;
using System.Linq;
using Space_Server.game;
using Space_Server.game.ship_components;
using Space_Server.server;
using Space_Server.utility;

namespace Space_Server {
    internal static class Program {
        public static void Main(string[] args) {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Log.Print($"Process x64 {Environment.Is64BitProcess}");
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
            
            // const int sellSize = 10;
            // var front = new PersonArena();
            // var back = new PersonArena();
            // var pvp = new PvpArena();
            // front.Arena[0, 0] = new SpaceShip(HullType.Diversant);
            // front.Arena[0, 1] = new SpaceShip(HullType.Diversant);
            // front.Arena[0, 2] = new SpaceShip(HullType.Diversant);
            // front.Arena[0, 3] = new SpaceShip(HullType.Diversant);
            // front.Arena[0, 4] = new SpaceShip(HullType.Diversant);
            // front.Arena[0, 5] = new SpaceShip(HullType.Diversant);
            // back.Arena[1, 1] = new SpaceShip(HullType.Diversant);
            // back.Arena[0, 4] = new SpaceShip(HullType.Diversant);
            // pvp.LocateFrontPlayer(front);
            // pvp.LocateBackPlayer(back);
            // var fightShips = GenerateFightShips(pvp.Arena);
            // foreach (var fightShip in fightShips) {
            //     fightShip.CalcPosition(pvp, sellSize);
            //     Console.WriteLine($"{fightShip.Ship.Hull} {fightShip.X} {fightShip.Y} {fightShip.Rotate}");
            // }
            // PrintMas(pvp.Arena);
            // Console.WriteLine();
            // PrintPvp(pvp.Arena, sellSize, fightShips);
        }

        private static void PrintMas(SpaceShip[,] matrix) {
            var w = matrix.GetLength(0); // width
            var h = matrix.GetLength(1); // height
            for (var x = 0; x < w; x++) {
                for (var y = 0; y < h; y++) {
                    Console.Write($"{matrix[x, y]?.Hull} {(y < h - 1 ? "|" : "")} ");
                }
                Console.WriteLine();
            }
        }

        private static List<FightShip> GenerateFightShips(SpaceShip[,] matrix) {
            var result = new List<FightShip>();
            var w = matrix.GetLength(0); // width
            var h = matrix.GetLength(1); // height
            for (var x = 0; x < w; x++) {
                for (var y = 0; y < h; y++) {
                    if (matrix[x, y] != null) {
                        result.Add(new FightShip(matrix[x, y]));
                    }
                }
            }
            return result;
        }

        private static void PrintPvp(SpaceShip[,] matrix, int sellSize, List<FightShip> fightShips) {
            var w = matrix.GetLength(0); // width
            var h = matrix.GetLength(1); // height
            for (var x = 0; x < w * sellSize; x++) {
                for (var y = 0; y < h * sellSize; y++) {
                    var hasShip = fightShips.Where(ship => Math.Abs(ship.X - x) < float.Epsilon && Math.Abs(ship.Y - y) < float.Epsilon).ToList();
                    if (hasShip.Count > 0) {
                        Console.Write(Math.Abs(hasShip[0].Rotate) < float.Epsilon ? "↑" : "↓");
                    } else {
                        Console.Write(".");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}