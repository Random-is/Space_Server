using System.Collections.Generic;
using Space_Server.game.ship_components;

namespace Space_Server.game {
    public class SpaceShip {
        public HullType Hull { get; set; }
        public ShipComponentType Gun { get; set; }
        public ShipComponentType Reactor { get; set; }
        public ShipComponentType Shell { get; set; }

        public SpaceShip(HullType hull) {
            Hull = hull;
        }
    }
}