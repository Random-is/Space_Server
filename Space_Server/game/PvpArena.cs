using System;

namespace Space_Server.game {
    public class PvpArena {
        public SpaceShip[,] Arena { get; } = new SpaceShip[6, 6];
        public int SellSize { get; } = 500;

        public void LocateFrontPlayer(PersonArena personArena) {
            var arena = personArena.Arena;
            for (var i = 0; i < arena.GetLength(0); i++) {
                for (var j = 0; j < arena.GetLength(1); j++) {
                    Arena[Arena.GetLength(0) - arena.GetLength(0) + i, j] = arena[i, j];
                }
            }
        }
        
        public void LocateBackPlayer(PersonArena personArena) {
            var arena = personArena.Arena;
            for (var i = 0; i < arena.GetLength(0); i++) {
                for (var j = 0; j < arena.GetLength(1); j++) {
                    Console.WriteLine($"{i}, {j} <= {arena.GetLength(0) - 1 - i} {arena.GetLength(1) - 1 - j}");
                    Arena[i, j] = arena[arena.GetLength(0) - 1 - i, arena.GetLength(1) - 1 - j];
                }
            }
        }
        
        public float CalcDistance(SpaceShip firstShip, SpaceShip secondShip) {
            return 0;
        }
    }
}