namespace Game_Components.ship {
    public enum ShipClassName {
        Armored,
        Bomber,
        Fighter,
        Saboteur,
        Technodroid
    }

    public class ShipClass {
        public ShipClassName Name { get; }
        public ShipClass(ShipClassName name) {
            Name = name;
        }
    }
}