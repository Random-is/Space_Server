using System;
using System.Collections.Generic;
using Game_Elements.fight;

namespace Game_Elements.ship.ship_part {
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
        Gun5,
        Gun6,
        Gun7,
        Gun8,
        Gun9,
        Gun10,
        Gun11,
        Gun12,
        Gun13,
        Gun14,
        Gun15,
        Gun16,
        Gun17,
        Gun18,
        Facing1,
        Facing2,
        Facing3,
        Facing4,
        Facing5,
        Facing6,
        Facing7,
        Rector1,
        Rector2,
        Rector3,
        Rector4,
    }

    public class ShipPart {
        public ShipPartType Type { get; }
        public ShipPartName Name { get; }
        public TierName TierName { get; }
        
        public Dictionary<ShipParameterName, float> Parameters { get; }
        public ShipPartSpell Spell { get; }
        public Dictionary<ShipClassName, int> Classes { get; }

        public ShipPart(
            ShipPartName name,
            ShipPartType type,
            int rank,
            TierName tierName,
            Dictionary<ShipClassName, int> classes,
            ShipPartSpell spell,
            Dictionary<ShipParameterName, float> parameters
        ) {
            Name = name;
            Type = type;
            Rank = rank;
            TierName = tierName;
            Classes = classes;
            Spell = spell;
            Parameters = parameters;
        }

        public int Rank { get; set; }

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