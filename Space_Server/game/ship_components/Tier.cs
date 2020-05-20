using System.Collections.Concurrent;

namespace Space_Server.game.ship_components {
    public class Tier {
        public static readonly ConcurrentDictionary<int, int[]> ChancesByLvl = new ConcurrentDictionary<int, int[]> {
            [1] = new[] {100, 0, 0, 0, 0},
            [2] = new[] {50, 50, 0, 0, 0},
            [3] = new[] {34, 33, 33, 0, 0},
            [4] = new[] {25, 25, 25, 25, 0},
            [5] = new[] {35, 20, 20, 20, 5}
        };
        public static readonly Tier One = new Tier(0, 100, 1);
        public static readonly Tier Two = new Tier(1, 8, 2);
        public static readonly Tier Three = new Tier(2, 6, 3);
        public static readonly Tier Four = new Tier(3, 4, 4);
        public static readonly Tier Five = new Tier(4, 2, 5);
        public int Index { get; }
        public int Count { get; }
        public int Cost { get; }

        private Tier(int index, int count, int cost) {
            Index = index;
            Count = count;
            Cost = cost;
        }
    }
}