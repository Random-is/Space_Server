using System.Collections.Generic;

namespace Game_Elements.ship.ship_part {
    public enum ShipHullName {
        Armored,
        Bomber,
        Fighter,
        Saboteur,
        Technodroid
    }

    public class ShipHull {
        public Dictionary<ShipParameterName, float> Parameters { get; }
        public ShipHullName Name { get; }
        public ShipHull(ShipHullName name, Dictionary<ShipParameterName, float> parameters) {
            Name = name;
            Parameters = parameters;
        }
    }
}