using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
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
            
            
            var mainPlayer = new GamePlayer();
            var opponentPlayer = new GamePlayer();
            
            mainPlayer.Reset();
            mainPlayer.AddShip(ShipHullName.Armored);
            mainPlayer.AddShip(ShipHullName.Bomber);
            mainPlayer.AddShip(ShipHullName.Technodroid);
            mainPlayer.ShipReposition(mainPlayer.Ships[0], 0, 5);

            
            opponentPlayer.Reset();
            opponentPlayer.AddShip(ShipHullName.Saboteur);
            opponentPlayer.AddShip(ShipHullName.Fighter);
            // opponentPlayer.ShipReposition(1, 2, 4);
            
            var fightArena = new FightArena(mainPlayer, opponentPlayer);
            
            PrintMatrix(mainPlayer.PlayerArena.Arena);
            Console.WriteLine();
            PrintMatrix(opponentPlayer.PlayerArena.Arena);
            Console.WriteLine();
            fightArena.FightShips.ForEach(ship => {
                Console.Write(ship.Position + " ");
                Console.WriteLine();
            });
            Console.WriteLine(fightArena);
            Console.ReadLine();
            
            var fightResult = Fight.CalcWinner(
                fightArena, 
                10, 
                new Random(), 
                moveAction: (ship, arena, tickRate) => {
                    Console.Clear();
                    Console.WriteLine(arena);
                    Thread.SpinWait(450000);
                },
                attackAction: (ship, arena, tickRate) => {
                    Console.Clear();
                    Console.WriteLine(arena);
                    Thread.SpinWait(450000);
                });
            Console.WriteLine($"Winner {fightResult.Winner.Nickname}");
            Console.ReadLine();

            // var target = new Vector2(10, 10);
            // var pos = new Vector2(0, 0);
            // while (pos != target) {
            //     Console.WriteLine(pos);
            //     pos = Vector2Ex.MoveTowards(pos, target, 1f);
            // }
        }

        private static void PrintMatrix(Ship[,] matrix) {
            for (var y = 0; y < matrix.GetLength(0); y++) {
                for (var x = 0; x < matrix.GetLength(1); x++) {
                    Console.Write($"{matrix[y, x]?.Hull.Name} {(x < matrix.GetLength(1) - 1 ? "|" : "")} ");
                }
                Console.WriteLine();
            }
        }
    }
}