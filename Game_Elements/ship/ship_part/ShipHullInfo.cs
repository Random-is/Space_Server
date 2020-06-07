using System.Collections.Generic;

namespace Game_Components.ship.ship_part {
    public static class ShipHullInfo {
        public static readonly Dictionary<ShipHullName, ShipHull> All = new Dictionary<ShipHullName, ShipHull> {
            [ShipHullName.Armored] = new ShipHull(ShipHullName.Armored),
            [ShipHullName.Bomber] = new ShipHull(ShipHullName.Bomber),
            [ShipHullName.Fighter] = new ShipHull(ShipHullName.Fighter),
            [ShipHullName.Saboteur] = new ShipHull(ShipHullName.Saboteur),
            [ShipHullName.Technodroid] = new ShipHull(ShipHullName.Technodroid)
        };
    }
}