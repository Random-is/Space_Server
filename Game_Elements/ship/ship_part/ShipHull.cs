namespace Game_Elements.ship.ship_part {
    public enum ShipHullName {
        Armored,
        Bomber,
        Fighter,
        Saboteur,
        Technodroid
    }

    public class ShipHull {
        public ShipHullName Name { get; }
        public ShipHull(ShipHullName name) {
            Name = name;
        }
    }
}