using Game_Elements.ship;

namespace Game_Elements.arena {
    public class PvpArena {
        public const int YSize = 6;
        public const int XSize = PersonArena.XSize;
        public Ship[,] Arena { get; } = new Ship[YSize, XSize];

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
                    Arena[i, j] = arena[arena.GetLength(0) - 1 - i, arena.GetLength(1) - 1 - j];
                }
            }
        }
    }
}