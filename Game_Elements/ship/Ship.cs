using System;
using System.Collections.Generic;
using System.Linq;
using Game_Elements.ship.ship_part;
using Game_Elements.utility;

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

        public List<ShipClassName> GetActiveClasses() {
            var shipClasses = new Dictionary<ShipClassName, int>();
            foreach (var component in Parts.Values.Where(component => component != null)) {
                shipClasses.AppendWithAddition(component.Classes);
            }
            return (from shipClass in shipClasses
                    where shipClass.Value >= 8
                    select shipClass.Key).ToList();
        }
        
        public Dictionary<ShipClassName, int> GetAllClasses() {
            var shipClasses = new Dictionary<ShipClassName, int>();
            foreach (var component in Parts.Values.Where(component => component != null)) {
                shipClasses.AppendWithAddition(component.Classes);
            }
            foreach (var shipClass in shipClasses.Keys.ToList().Where(shipClass => shipClasses[shipClass] > 8)) {
                shipClasses[shipClass] = 8;
            }
            return shipClasses;
        }

        public Dictionary<ShipParameterName, float> GetParameters() {
            var shipParameters = new Dictionary<ShipParameterName, float>();
            shipParameters.AppendWithAddition(Hull.Parameters);
            foreach (var component in Parts.Values.Where(component => component != null)) {
                shipParameters.AppendWithAddition(component.Parameters);
            }
            return shipParameters;
        }
    }
}