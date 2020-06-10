using System.Collections.Generic;
using System.Linq;
using Game_Elements.ship.ship_part;

namespace Game_Elements.ship {
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

        public List<ShipClassName> GetClasses() {
            var shipClasses = new Dictionary<ShipClassName, int>();
            foreach (var component in Parts.Values.Where(component => component != null)) {
                foreach (var (shipClassName, count) in component.Classes) {
                    if (shipClasses.ContainsKey(shipClassName)) {
                        shipClasses[shipClassName] += count;
                    } else {
                        shipClasses[shipClassName] = count;
                    }
                }
            }
            return (from shipClass in shipClasses
                    where shipClass.Value >= 8
                    select shipClass.Key).ToList();
        }
    }
}