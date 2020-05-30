using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Space_Server.game.ship_components {
    public enum ShipComponentType {
        Gun,
        Facing,
        Reactor
    }

    public static class ShipComponentName {
        public const string Gun1 = "Gun1";
        public const string Gun2 = "Gun2";
        public const string Gun3 = "Gun3";
        public const string Gun4 = "Gun4";
        public const string Gun5 = "Gun5";
    }

    public class ShipComponent {
        public ShipComponentType Type { get; }
        public string Name { get; }
        public Tier Tier { get; }
        public Dictionary<ShipClassType, int> Classes { get; }
        public ComponentSpell Spell { get; }

        public ShipComponent(
            ShipComponentType type,
            string name,
            Tier tier,
            Dictionary<ShipClassType, int> classes,
            ComponentSpell spell
        ) {
            Type = type;
            Name = name;
            Tier = tier;
            Classes = classes;
            Spell = spell;
        }

        public override string ToString() {
            return Name;
        }
    }

    public class ComponentSpell {
        public Action<PvpArena, FightShip, Random> Spell { get; }
        public float SpellCastSeconds { get; }

        public ComponentSpell(Action<PvpArena, FightShip, Random> spell, float spellCastSeconds) {
            Spell = spell;
            SpellCastSeconds = spellCastSeconds;
        }
    }
}