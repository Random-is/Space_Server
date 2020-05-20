using System.Collections.Generic;
using Space_Server.game.ship_components;

namespace Space_Server.game {
    public class SpaceShip {
        public Hull Hull { get; set; }
        public Gun Gun { get; set; }
        public Reactor Reactor { get; set; }
        public Shell Shell { get; set; }

        public SpaceShip(Hull hull) {
            Hull = hull;
        }
    }
}