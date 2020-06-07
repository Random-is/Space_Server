using System;
using System.Collections.Generic;
using Game_Components.fight;

namespace Game_Components.ship.ship_part {
    public enum ShipPartType {
        Gun,
        Facing,
        Reactor
    }

    public enum ShipPartName {
        Gun1,
        Gun2,
        Gun3,
        Gun4,
        Gun5
    }

    public class ShipPart {
        public ShipPartType Type { get; }
        public ShipPartName Name { get; }
        public TierName TierName { get; }
        public Dictionary<ShipClassName, int> Classes { get; }
        public ShipPartSpell Spell { get; }

        public ShipPart(
            ShipPartType type,
            ShipPartName name,
            TierName tierName,
            Dictionary<ShipClassName, int> classes,
            ShipPartSpell spell
        ) {
            Type = type;
            Name = name;
            TierName = tierName;
            Classes = classes;
            Spell = spell;
        }

        public override string ToString() {
            return Name.ToString();
        }
    }

    public class ShipPartSpell {
        public Action<List<FightShip>, FightShip, Random> Spell { get; }
        public float SpellCastSeconds { get; }

        public ShipPartSpell(Action<List<FightShip>, FightShip, Random> spell, float spellCastSeconds) {
            Spell = spell;
            SpellCastSeconds = spellCastSeconds;
        }
    }
}