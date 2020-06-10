using System.Collections.Generic;

namespace Game_Elements.ship {
    public static class ShipClassInfo {
        public static readonly Dictionary<ShipClassName, ShipClass> All = new Dictionary<ShipClassName, ShipClass> {
            [ShipClassName.Armored] = new ShipClass(ShipClassName.Armored),
            [ShipClassName.Bomber] = new ShipClass(ShipClassName.Bomber),
            [ShipClassName.Fighter] = new ShipClass(ShipClassName.Fighter),
            [ShipClassName.Saboteur] = new ShipClass(ShipClassName.Saboteur),
            [ShipClassName.Technodroid] = new ShipClass(ShipClassName.Technodroid)
        };
    }
}