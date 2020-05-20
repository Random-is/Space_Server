using System;
using System.Collections.Generic;

namespace Space_Server.game.ship_components {
    
    public class ComponentBase {
        public string Name { get; }
        public Tier Tier { get; }
        public Dictionary<ShipClassType, int> Classes { get; }

        public ComponentBase(
            string name,
            Tier tier,
            Dictionary<ShipClassType, int> classes
        ) {
            Name = name;
            Tier = tier;
            Classes = classes;
        }

        public override string ToString() {
            return Name;
        }
    }
    
    public class Gun : ComponentBase {
        public Action<PvpArena, SpaceShip> Spell { get; }
        public Gun(
            string name,
            Tier tier,
            Dictionary<ShipClassType, int> classes,
            Action<PvpArena, SpaceShip> spell
        ) : base(name, tier, classes) {
            Spell = spell;
        }
    }
    
    public class Reactor : ComponentBase {
        public Reactor(string name, Tier tier, Dictionary<ShipClassType, int> classes) : base(name, tier, classes) {
        }
    }
    
    public class Shell : ComponentBase {
        public Shell(string name, Tier tier, Dictionary<ShipClassType, int> classes) : base(name, tier, classes) {
        }
    }
}