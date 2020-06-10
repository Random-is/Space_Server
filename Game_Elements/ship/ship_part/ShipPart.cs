﻿using System;
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
        Facing1,
        Facing2,
        Facing3,
        Rector1,
        Rector2,
        Rector3
    }

    public class ShipPart {
        public ShipPartType Type { get; }
        public ShipPartName Name { get; }
        public TierName TierName { get; }
        
        public Dictionary<ShipParameterName, float> Parameters { get; }
        public ShipPartSpell Spell { get; }
        public Dictionary<ShipClassName, int> Classes { get; }

        public ShipPart(
            ShipPartType type,
            ShipPartName name,
            TierName tierName,
            Dictionary<ShipClassName, int> classes,
            ShipPartSpell spell,
            Dictionary<ShipParameterName, float> parameters
        ) {
            Type = type;
            Name = name;
            TierName = tierName;
            Classes = classes;
            Spell = spell;
            Parameters = parameters;
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