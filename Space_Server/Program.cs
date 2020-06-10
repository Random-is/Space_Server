using System;
using System.Collections.Generic;
using System.Linq;
using Game_Elements;
using Game_Elements.arena;
using Game_Elements.fight;
using Game_Elements.ship;
using Game_Elements.ship.ship_part;
using Game_Elements.utility;
using Space_Server.game;
using Space_Server.server;
using Space_Server.utility;

namespace Space_Server {
    internal static class Program {
        public static void Main(string[] args) {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            // Log.Print($"Process x64 {Environment.Is64BitProcess}");
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
            
            
            // const int sellSize = 5;
            // var mainPlayer = new GamePlayer();
            // var opponentPlayer = new GamePlayer();
            //
            // mainPlayer.Reset();
            // mainPlayer.AddShip(ShipHullName.Armored);
            // // mainPlayer.ShipReposition(0, 2, 0);
            // mainPlayer.AddShip(ShipHullName.Bomber);
            // mainPlayer.AddShip(ShipHullName.Technodroid);
            //
            // opponentPlayer.Reset();
            // opponentPlayer.AddShip(ShipHullName.Saboteur);
            // opponentPlayer.AddShip(ShipHullName.Fighter);
            // // opponentPlayer.ShipReposition(1, 2, 4);
            //
            // var pvpArena = new PvpArena();
            // pvpArena.LocateFrontPlayer(mainPlayer.PersonArena);
            // pvpArena.LocateBackPlayer(opponentPlayer.PersonArena);
            //
            // PrintMas(mainPlayer.PersonArena.Arena);
            // Console.WriteLine();
            // PrintMas(opponentPlayer.PersonArena.Arena);
            // Console.WriteLine();
            // PrintMas(pvpArena.Arena);
            // Console.WriteLine();
            // Console.ReadLine();
            //
            // var fightResult = Fight.CalcWinner(
            //     mainPlayer, 
            //     opponentPlayer, 
            //     pvpArena, 
            //     10, 
            //     new Random(), 
            //     moveAction: (ship, ships, tickRate) => {
            //         PrintPvp(PvpArena.YSize, PvpArena.XSize, sellSize, ships);
            //     },
            //     attackAction: (ship, ships, tickRate) => {
            //         PrintPvp(PvpArena.YSize, PvpArena.XSize, sellSize, ships);
            //     });
            // Console.WriteLine($"Winner {fightResult.Winner.Nickname}");
            // Console.ReadLine();
            
            var shipClassesDict = new Dictionary<ShipClassName, int>();
            var shipClassesDict1 = new Dictionary<ShipClassName, int> {
                [ShipClassName.Armored] = 1,
                [ShipClassName.Bomber] = 1
            };
            var shipClassesDict2 = new Dictionary<ShipClassName, int> {
                [ShipClassName.Armored] = 2,
                [ShipClassName.Technodroid] = 3
            };
            var result = shipClassesDict1.ConcatWithAddition(shipClassesDict2);
            foreach (var (key, value) in result) {
                Console.WriteLine($"{key} {value}");
            }
        }

        private static void PrintMas(Ship[,] matrix) {
            for (var y = 0; y < matrix.GetLength(0); y++) {
                for (var x = 0; x < matrix.GetLength(1); x++) {
                    Console.Write($"{matrix[y, x]?.Hull.Name} {(x < matrix.GetLength(1) - 1 ? "|" : "")} ");
                }
                Console.WriteLine();
            }
        }

        private static List<FightShip> GenerateFightShips(Ship[,] matrix) {
            var result = new List<FightShip>();
            for (var y = 0; y < matrix.GetLength(0); y++) {
                for (var x = 0; x < matrix.GetLength(1); x++) {
                    if (matrix[y, x] != null) {
                        result.Add(new FightShip(matrix[y, x], new IntVector2 {Y = y, X = x}, new GamePlayer()));
                    }
                }
            }
            return result;
        }

        private static void PrintPvp(int ySize, int xSize, int sellSize, List<FightShip> fightShips) {
            // Console.Clear();
            Console.WriteLine(string.Join(',', fightShips.Select(ship => ship.Hp)));
            for (var y = 0; y < ySize * sellSize; y++) {
                for (var x = 0; x < xSize * sellSize; x++) {
                    var hasShip = fightShips.Where(ship => Math.Abs(MathF.Round(ship.X) - x) < float.Epsilon && Math.Abs(MathF.Round(ship.Y) - y) < float.Epsilon).ToList();
                    if (hasShip.Count > 0) {
                        Console.Write(Math.Abs(hasShip[0].RotateAngle) < 90 ? "↑" : "↓");
                    } else {
                        Console.Write(".");
                    }
                }
                Console.WriteLine();
            }
            // Console.ReadLine();
        }
    }
}