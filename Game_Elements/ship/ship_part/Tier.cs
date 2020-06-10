namespace Game_Elements.ship.ship_part {
    public enum TierName {
        One,
        Two,
        Three,
        Four,
        Five
    }

    public class Tier {
        public int Count { get; }
        public int Cost { get; }

        public Tier(int count, int cost) {
            Count = count;
            Cost = cost;
        }
    }
}