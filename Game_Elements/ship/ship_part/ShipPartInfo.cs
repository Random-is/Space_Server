using System.Collections.Generic;

namespace Game_Elements.ship.ship_part {
    public static class ShipPartInfo {
        static ShipPartInfo() {
            BaseRank = new Dictionary<ShipPartName, ShipPart>();
            InitBaseRank();
            T2Rank = new Dictionary<ShipPartName, ShipPart>();
            InitT2Rank();
        }
        
        public static readonly Dictionary<ShipPartName, ShipPart> BaseRank;
        public static readonly Dictionary<ShipPartName, ShipPart> T2Rank;

        private static void AddBaseRankShipPart(
            ShipPartName name,
            ShipPartType type,
            TierName tierName,
            Dictionary<ShipClassName, int> classes,
            ShipPartSpell spell,
            Dictionary<ShipParameterName, float> parameters
        ) {
            BaseRank.Add(name, new ShipPart(name, type, 1, tierName, classes, spell, parameters));
        }
        
        private static void AddT2RankShipPart(
            ShipPartName name,
            ShipPartType type,
            TierName tierName,
            Dictionary<ShipClassName, int> classes,
            ShipPartSpell spell,
            Dictionary<ShipParameterName, float> parameters
        ) {
            T2Rank.Add(name, new ShipPart(name, type, 2, tierName, classes, spell, parameters));
        }

        private static void InitBaseRank() {
            AddBaseRankShipPart(
                ShipPartName.Gun1,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Technodroid] = 3
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => {
                        ship.Target.Hp -= 5;
                    },
                    2), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 300,
                    [ShipParameterName.AttackDamage] = 120,
                    [ShipParameterName.MagicDamage] = 10,
                    [ShipParameterName.Armor] = 20,
                    [ShipParameterName.MagicResist] = 30,
                    [ShipParameterName.AttackRange] = 2,
                    [ShipParameterName.AttackSpeed] = 0.4f
                });
            AddBaseRankShipPart(ShipPartName.Gun2,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 2
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 900,
                    [ShipParameterName.AttackDamage] = 100,
                    [ShipParameterName.MagicDamage] = 0,
                    [ShipParameterName.Armor] = 20,
                    [ShipParameterName.MagicResist] = 20,
                    [ShipParameterName.AttackRange] = 4,
                    [ShipParameterName.AttackSpeed] = 0.68f
                });
            AddBaseRankShipPart(ShipPartName.Gun3,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Bomber] = 4,
                    [ShipClassName.Armored] = 2
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 300,
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 40,
                    [ShipParameterName.MagicResist] = 10,
                    [ShipParameterName.AttackRange] = 0,
                    [ShipParameterName.AttackSpeed] = 0.2f
                });
            AddBaseRankShipPart(ShipPartName.Gun4,
                ShipPartType.Gun,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Armored] = 2,
                    [ShipClassName.Bomber] = 4,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun5,
                ShipPartType.Gun,
                TierName.Two,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Armored] = 3,
                    [ShipClassName.Bomber] = 3,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun6,
                ShipPartType.Gun,
                TierName.Two,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Technodroid] = 1,
                    [ShipClassName.Saboteur] = 4,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun7,
                ShipPartType.Gun,
                TierName.Two,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Armored] = 3,
                    [ShipClassName.Saboteur] = 5,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun8,
                ShipPartType.Gun,
                TierName.Three,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 6,
                    [ShipClassName.Saboteur] = 6,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun9,
                ShipPartType.Gun,
                TierName.Three,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Bomber] = 1,
                    [ShipClassName.Armored] = 2,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun10,
                ShipPartType.Gun,
                TierName.Three,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 4,
                    [ShipClassName.Armored] = 4,
                    [ShipClassName.Technodroid] = 4,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun11,
                ShipPartType.Gun,
                TierName.Four,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Technodroid] = 2,
                    [ShipClassName.Armored] = 2,
                    [ShipClassName.Fighter] = 2,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun12,
                ShipPartType.Gun,
                TierName.Four,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Armored] = 1,
                    [ShipClassName.Bomber] = 3,
                    [ShipClassName.Saboteur] = 3,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun13,
                ShipPartType.Gun,
                TierName.Five,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 5,
                    [ShipClassName.Technodroid] = 5,
                    [ShipClassName.Saboteur] = 3,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun14,
                ShipPartType.Gun,
                TierName.Five,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 5,
                    [ShipClassName.Technodroid] = 5,
                    [ShipClassName.Saboteur] = 3,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun15,
                ShipPartType.Gun,
                TierName.Five,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 5,
                    [ShipClassName.Technodroid] = 5,
                    [ShipClassName.Saboteur] = 3,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun16,
                ShipPartType.Gun,
                TierName.Five,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 5,
                    [ShipClassName.Technodroid] = 5,
                    [ShipClassName.Saboteur] = 3,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun17,
                ShipPartType.Gun,
                TierName.Five,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 5,
                    [ShipClassName.Technodroid] = 5,
                    [ShipClassName.Saboteur] = 3,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Gun18,
                ShipPartType.Gun,
                TierName.Five,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 5,
                    [ShipClassName.Technodroid] = 5,
                    [ShipClassName.Saboteur] = 3,
                },
                new ShipPartSpell((fightShips, ship, random) => { ship.Target.Hp -= 5; }, 1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.Health] = 150,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.Armor] = 30,
                    [ShipParameterName.MagicResist] = 30,
                });
            AddBaseRankShipPart(ShipPartName.Rector1,
                ShipPartType.Reactor,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Technodroid] = 4
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.AttackRange] = 1,
                    [ShipParameterName.AttackSpeed] = 0.2f
                });
            AddBaseRankShipPart(ShipPartName.Rector2,
                ShipPartType.Reactor,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Bomber] = 2
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.AttackDamage] = 40,
                    [ShipParameterName.MagicDamage] = 120,
                    [ShipParameterName.AttackRange] = 0,
                    [ShipParameterName.AttackSpeed] = 0.2f
                });
            AddBaseRankShipPart(ShipPartName.Rector3,
                ShipPartType.Reactor,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 8
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 420,
                    [ShipParameterName.AttackDamage] = 46,
                    [ShipParameterName.MagicDamage] = 200,
                });
            AddBaseRankShipPart(ShipPartName.Rector4,
                ShipPartType.Reactor,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 8
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 420,
                    [ShipParameterName.AttackDamage] = 46,
                    [ShipParameterName.MagicDamage] = 200,
                });
            AddBaseRankShipPart(ShipPartName.Facing1,
                ShipPartType.Facing,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Fighter] = 4,
                    [ShipClassName.Saboteur] = 2
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 400,
                    [ShipParameterName.Armor] = 20,
                    [ShipParameterName.MagicResist] = 20,
                });
            AddBaseRankShipPart(ShipPartName.Facing2,
                ShipPartType.Facing,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Armored] = 4,
                    [ShipClassName.Technodroid] = 2
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 500,
                    [ShipParameterName.Armor] = 50,
                    [ShipParameterName.MagicResist] = 34,
                });
            AddBaseRankShipPart(ShipPartName.Facing3,
                ShipPartType.Facing,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Bomber] = 4
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 300,
                    [ShipParameterName.Armor] = 32,
                    [ShipParameterName.MagicResist] = 22,
                });
            AddBaseRankShipPart(ShipPartName.Facing4,
                ShipPartType.Facing,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Bomber] = 4
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 300,
                    [ShipParameterName.Armor] = 32,
                    [ShipParameterName.MagicResist] = 22,
                });
            AddBaseRankShipPart(ShipPartName.Facing5,
                ShipPartType.Facing,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Bomber] = 4
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 300,
                    [ShipParameterName.Armor] = 32,
                    [ShipParameterName.MagicResist] = 22,
                });
            AddBaseRankShipPart(ShipPartName.Facing6,
                ShipPartType.Facing,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Bomber] = 4
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 300,
                    [ShipParameterName.Armor] = 32,
                    [ShipParameterName.MagicResist] = 22,
                });
            AddBaseRankShipPart(ShipPartName.Facing7,
                ShipPartType.Facing,
                TierName.One,
                new Dictionary<ShipClassName, int> {
                    [ShipClassName.Bomber] = 4
                },
                new ShipPartSpell(
                    (fightShips, ship, random) => { ship.Target.Hp -= 5; },
                    1), new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 300,
                    [ShipParameterName.Armor] = 32,
                    [ShipParameterName.MagicResist] = 22,
                });
        }
        
        private static void InitT2Rank() {
            foreach (var shipPart in BaseRank) {
                AddT2RankShipPart(shipPart.Key, shipPart.Value.Type, shipPart.Value.TierName, shipPart.Value.Classes, shipPart.Value.Spell, shipPart.Value.Parameters);
            }
            // AddT2RankShipPart(
            //     ShipPartName.Gun1,
            //     ShipPartType.Gun,
            //     TierName.One,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Technodroid] = 3
            //     },
            //     new ShipPartSpell(
            //         (fightShips, ship, random) => { },
            //         1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.Health] = 300,
            //         [ShipParameterName.AttackDamage] = 120,
            //         [ShipParameterName.MagicDamage] = 10,
            //         [ShipParameterName.Armor] = 20,
            //         [ShipParameterName.MagicResist] = 30,
            //         [ShipParameterName.AttackRange] = 2,
            //         [ShipParameterName.AttackSpeed] = 0.4f
            //     });
            // AddT2RankShipPart(ShipPartName.Gun2,
            //     ShipPartType.Gun,
            //     TierName.One,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Fighter] = 2
            //     },
            //     new ShipPartSpell(
            //         (fightShips, ship, random) => { },
            //         1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.Health] = 900,
            //         [ShipParameterName.AttackDamage] = 100,
            //         [ShipParameterName.MagicDamage] = 0,
            //         [ShipParameterName.Armor] = 20,
            //         [ShipParameterName.MagicResist] = 20,
            //         [ShipParameterName.AttackRange] = 4,
            //         [ShipParameterName.AttackSpeed] = 0.68f
            //     });
            // AddT2RankShipPart(ShipPartName.Gun3,
            //     ShipPartType.Gun,
            //     TierName.One,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Bomber] = 4,
            //         [ShipClassName.Armored] = 2
            //     },
            //     new ShipPartSpell(
            //         (fightShips, ship, random) => { },
            //         1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.Health] = 300,
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 40,
            //         [ShipParameterName.MagicResist] = 10,
            //         [ShipParameterName.AttackRange] = 0,
            //         [ShipParameterName.AttackSpeed] = 0.2f
            //     });
            // AddT2RankShipPart(ShipPartName.Facing1,
            //     ShipPartType.Facing,
            //     TierName.One,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Fighter] = 4,
            //         [ShipClassName.Saboteur] = 2
            //     },
            //     new ShipPartSpell(
            //         (fightShips, ship, random) => { },
            //         1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.Health] = 400,
            //         [ShipParameterName.Armor] = 20,
            //         [ShipParameterName.MagicResist] = 20,
            //     });
            // AddT2RankShipPart(ShipPartName.Facing2,
            //     ShipPartType.Facing,
            //     TierName.One,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Armored] = 4,
            //         [ShipClassName.Technodroid] = 2
            //     },
            //     new ShipPartSpell(
            //         (fightShips, ship, random) => { },
            //         1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.Health] = 500,
            //         [ShipParameterName.Armor] = 50,
            //         [ShipParameterName.MagicResist] = 34,
            //     });
            // AddT2RankShipPart(ShipPartName.Facing3,
            //     ShipPartType.Facing,
            //     TierName.One,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Bomber] = 4
            //     },
            //     new ShipPartSpell(
            //         (fightShips, ship, random) => { },
            //         1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.Health] = 300,
            //         [ShipParameterName.Armor] = 32,
            //         [ShipParameterName.MagicResist] = 22,
            //     });
            // AddT2RankShipPart(ShipPartName.Rector1,
            //     ShipPartType.Reactor,
            //     TierName.One,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Technodroid] = 4
            //     },
            //     new ShipPartSpell(
            //         (fightShips, ship, random) => { },
            //         1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.AttackRange] = 1,
            //         [ShipParameterName.AttackSpeed] = 0.2f
            //     });
            // AddT2RankShipPart(ShipPartName.Rector2,
            //     ShipPartType.Reactor,
            //     TierName.One,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Bomber] = 2
            //     },
            //     new ShipPartSpell(
            //         (fightShips, ship, random) => { },
            //         1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.AttackRange] = 0,
            //         [ShipParameterName.AttackSpeed] = 0.2f
            //     });
            // AddT2RankShipPart(ShipPartName.Rector3,
            //     ShipPartType.Reactor,
            //     TierName.One,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Fighter] = 8
            //     },
            //     new ShipPartSpell(
            //         (fightShips, ship, random) => { },
            //         1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.Health] = 420,
            //         [ShipParameterName.AttackDamage] = 46,
            //         [ShipParameterName.MagicDamage] = 200,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun4,
            //     ShipPartType.Gun,
            //     TierName.One,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Armored] = 2,
            //         [ShipClassName.Bomber] = 4,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun5,
            //     ShipPartType.Gun,
            //     TierName.Two,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Armored] = 3,
            //         [ShipClassName.Bomber] = 3,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun6,
            //     ShipPartType.Gun,
            //     TierName.Two,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Technodroid] = 1,
            //         [ShipClassName.Saboteur] = 4,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun7,
            //     ShipPartType.Gun,
            //     TierName.Two,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Armored] = 3,
            //         [ShipClassName.Saboteur] = 5,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun8,
            //     ShipPartType.Gun,
            //     TierName.Three,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Fighter] = 6,
            //         [ShipClassName.Saboteur] = 6,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun9,
            //     ShipPartType.Gun,
            //     TierName.Three,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Bomber] = 1,
            //         [ShipClassName.Armored] = 2,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun10,
            //     ShipPartType.Gun,
            //     TierName.Three,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Fighter] = 4,
            //         [ShipClassName.Armored] = 4,
            //         [ShipClassName.Technodroid] = 4,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun11,
            //     ShipPartType.Gun,
            //     TierName.Four,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Technodroid] = 2,
            //         [ShipClassName.Armored] = 2,
            //         [ShipClassName.Fighter] = 2,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun12,
            //     ShipPartType.Gun,
            //     TierName.Four,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Armored] = 1,
            //         [ShipClassName.Bomber] = 3,
            //         [ShipClassName.Saboteur] = 3,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun13,
            //     ShipPartType.Gun,
            //     TierName.Five,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Fighter] = 5,
            //         [ShipClassName.Technodroid] = 5,
            //         [ShipClassName.Saboteur] = 3,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun14,
            //     ShipPartType.Gun,
            //     TierName.Five,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Fighter] = 5,
            //         [ShipClassName.Technodroid] = 5,
            //         [ShipClassName.Saboteur] = 3,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun15,
            //     ShipPartType.Gun,
            //     TierName.Five,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Fighter] = 5,
            //         [ShipClassName.Technodroid] = 5,
            //         [ShipClassName.Saboteur] = 3,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun16,
            //     ShipPartType.Gun,
            //     TierName.Five,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Fighter] = 5,
            //         [ShipClassName.Technodroid] = 5,
            //         [ShipClassName.Saboteur] = 3,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun17,
            //     ShipPartType.Gun,
            //     TierName.Five,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Fighter] = 5,
            //         [ShipClassName.Technodroid] = 5,
            //         [ShipClassName.Saboteur] = 3,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
            // AddT2RankShipPart(ShipPartName.Gun18,
            //     ShipPartType.Gun,
            //     TierName.Five,
            //     new Dictionary<ShipClassName, int> {
            //         [ShipClassName.Fighter] = 5,
            //         [ShipClassName.Technodroid] = 5,
            //         [ShipClassName.Saboteur] = 3,
            //     },
            //     new ShipPartSpell((list, ship, arg3) => { }, 1), new Dictionary<ShipParameterName, float> {
            //         [ShipParameterName.AttackDamage] = 40,
            //         [ShipParameterName.Health] = 150,
            //         [ShipParameterName.MagicDamage] = 120,
            //         [ShipParameterName.Armor] = 30,
            //         [ShipParameterName.MagicResist] = 30,
            //     });
        }
    }
}