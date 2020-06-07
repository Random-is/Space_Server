using System.Collections.Generic;
using Game_Components.ship.ship_part;

namespace Game_Components.ship {
    public class Ship {
        public readonly Dictionary<ShipPartType, ShipPart> Parts = new Dictionary<ShipPartType, ShipPart> {
            [ShipPartType.Gun] = null,
            [ShipPartType.Facing] = null,
            [ShipPartType.Reactor] = null,
        };
        public ShipHull Hull { get; }

        public Ship(ShipHull hull) {
            Hull = hull;
        }

        public ShipPart ChangeComponent(ShipPart shipPart) {
            var oldShipPart = Parts[shipPart.Type];
            Parts[shipPart.Type] = shipPart;
            return oldShipPart;
        }
    }
}