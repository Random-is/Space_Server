using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Space_Server.game.ship_components {
    public enum Tier {
        One,
        Two,
        Three,
        Four,
        Five
    }
    public class TierInfo {
        public static readonly ConcurrentDictionary<int, int[]> ChancesByLvl = new ConcurrentDictionary<int, int[]> {
            [1] = new[] {100, 0, 0, 0, 0},
            [2] = new[] {50, 50, 0, 0, 0},
            [3] = new[] {34, 33, 33, 0, 0},
            [4] = new[] {25, 25, 25, 25, 0},
            [5] = new[] {35, 20, 20, 20, 5}
        };
        public static readonly ImmutableDictionary<Tier, TierInfo> Tiers = new Dictionary<Tier, TierInfo> {
            [Tier.One] = new TierInfo(100, 1),
            [Tier.Two] = new TierInfo(8, 2),
            [Tier.Three] = new TierInfo(6, 3),
            [Tier.Four] = new TierInfo(4, 4),
            [Tier.Five] = new TierInfo(2, 5),
        }.ToImmutableDictionary();
        public int Count { get; }
        public int Cost { get; }

        private TierInfo(int count, int cost) {
            Count = count;
            Cost = cost;
        }
    }
}