using System.Collections.Generic;

namespace Game_Elements.ship.ship_part {
    public static class ShipPartInfo {
        static ShipPartInfo() {
            All = new Dictionary<ShipPartName, ShipPart>();
            AddShipPart(
                ShipPartName.Gun1,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Technodroid] = 3
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 300,
                    [ShipParameterName.AttackDamage] = 120,
                    [ShipParameterName.MagicDamage] = 10,
                    [ShipParameterName.Armor] = 20,
                    [ShipParameterName.MagicResist] = 30,
                    [ShipParameterName.AttackRange] = 2,
                    [ShipParameterName.AttackSpeed] = 0.4f
                });
            AddShipPart(ShipPartName.Gun2,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 2
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 900,
                    [ShipParameterName.AttackDamage] = 100,
                    [ShipParameterName.MagicDamage] = 0,
                    [ShipParameterName.Armor] = 20,
                    [ShipParameterName.MagicResist] = 20,
                    [ShipParameterName.AttackRange] = 4,
                    [ShipParameterName.AttackSpeed] = 0.68f
                });
            AddShipPart(ShipPartName.Gun3,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Bomber] = 4,
                    [ShipClassName.Armored] = 2
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 300,
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 40,
                    [ShipParameterName.MagicResist] = 10,
                    [ShipParameterName.AttackRange] = 0,
                    [ShipParameterName.AttackSpeed] = 0.2f
                });
            AddShipPart(ShipPartName.Facing1,
                ShipPartType.Facing,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 4,
                    [ShipClassName.Saboteur] = 2
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 400,
                    [ShipParameterName.Armor] = 20,
                    [ShipParameterName.MagicResist] = 20,
                });
            AddShipPart(ShipPartName.Facing2,
                ShipPartType.Facing,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Armored] = 4,
                    [ShipClassName.Technodroid] = 2
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 500,
                    [ShipParameterName.Armor] = 50,
                    [ShipParameterName.MagicResist] = 34,
                });
            AddShipPart(ShipPartName.Facing3,
                ShipPartType.Facing,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Bomber] = 4
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 300,
                    [ShipParameterName.Armor] = 32,
                    [ShipParameterName.MagicResist] = 22,
                });
            AddShipPart(ShipPartName.Rector1,
                ShipPartType.Reactor,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Technodroid] = 4
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.AttackRange] = 1,
                    [ShipParameterName.AttackSpeed] = 0.2f
                });
            AddShipPart(ShipPartName.Rector2,
                ShipPartType.Reactor,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Bomber] = 2
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.AttackRange] = 0,
                    [ShipParameterName.AttackSpeed] = 0.2f
                });
            AddShipPart(ShipPartName.Rector3,
                ShipPartType.Reactor,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 8
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 420,
                    [ShipParameterName.AttackDamage] = 46,
                    [ShipParameterName.MagicDamage] = 200,
                });
            AddShipPart(ShipPartName.Gun4,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Armored] = 2,
                    [ShipClassName.Bomber] = 4,
                },
                new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddShipPart(ShipPartName.Gun5,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Armored] = 3,
                    [ShipClassName.Bomber] = 3,
                },
                new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddShipPart(ShipPartName.Gun6,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Technodroid] = 1,
                    [ShipClassName.Saboteur] = 4,
                },
                new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddShipPart(ShipPartName.Gun7,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Armored] = 3,
                    [ShipClassName.Saboteur] = 5,
                },
                new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddShipPart(ShipPartName.Gun8,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 6,
                    [ShipClassName.Saboteur] = 6,
                },
                new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddShipPart(ShipPartName.Gun9,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Bomber] = 1,
                    [ShipClassName.Armored] = 2,
                },
                new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddShipPart(ShipPartName.Gun10,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 4,
                    [ShipClassName.Armored] = 4,
                    [ShipClassName.Technodroid] = 4,
                },
                new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddShipPart(ShipPartName.Gun11,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Technodroid] = 2,
                    [ShipClassName.Armored] = 2,
                    [ShipClassName.Fighter] = 2,
                },
                new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddShipPart(ShipPartName.Gun12,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Armored] = 1,
                    [ShipClassName.Bomber] = 3,
                    [ShipClassName.Saboteur] = 3,
                },
                new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddShipPart(ShipPartName.Gun13,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 5,
                    [ShipClassName.Technodroid] = 5,
                    [ShipClassName.Saboteur] = 3,
                },
                new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
        }
        
        public static readonly Dictionary<ShipPartName, ShipPart> All;

        private static void AddShipPart(
            ShipPartName name,
            ShipPartType type,
            TierName tierName,
            Dictionary<ShipClassName, int> classes,
            ShipPartSpell spell,
            Dictionary<ShipParameterName, float> parameters
        ) {
            All.Add(name, new ShipPart(name, type, tierName, classes, spell, parameters));
        }
    }
}