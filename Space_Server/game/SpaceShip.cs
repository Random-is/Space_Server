using System.Collections.Generic;
using Space_Server.game.ship_components;

namespace Space_Server.game {
    public class SpaceShip {
        public Dictionary<ShipComponentType, ShipComponent> Components = new Dictionary<ShipComponentType, ShipComponent> {
            [ShipComponentType.Gun] = null,
            [ShipComponentType.Facing] = null,
            [ShipComponentType.Reactor] = null,
        };
        public HullType Hull { get; set; }

        public SpaceShip(HullType hull) {
            Hull = hull;
        }
    }
}