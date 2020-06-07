using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Game_Components.ship.ship_part {
    public static class TierInfo {
        private static readonly ConcurrentDictionary<int, int[]> ChancesByLvl = new ConcurrentDictionary<int, int[]> {
            [1] = new[] {100, 0, 0, 0, 0},
            [2] = new[] {50, 50, 0, 0, 0},
            [3] = new[] {34, 33, 33, 0, 0},
            [4] = new[] {25, 25, 25, 25, 0},
            [5] = new[] {35, 20, 20, 20, 5}
        };

        private static readonly Dictionary<TierName, Tier> All = new Dictionary<TierName, Tier> {
            [TierName.One] = new Tier(100, 1),
            [TierName.Two] = new Tier(8, 2),
            [TierName.Three] = new Tier(6, 3),
            [TierName.Four] = new Tier(4, 4),
            [TierName.Five] = new Tier(2, 5),
        };

        public static Tier Get(TierName tierName) {
            return All[tierName];
        }

        public static int[] GetChances(int lvl) {
            return ChancesByLvl[lvl];
        }
    }
}