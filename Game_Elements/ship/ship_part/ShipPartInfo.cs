using System.Collections.Generic;

namespace Game_Elements.ship.ship_part {
    public static class ShipPartInfo {
        public static readonly Dictionary<ShipPartName, ShipPart> All =
            new Dictionary<ShipPartName, ShipPart> {
                [ShipPartName.Gun1] = new ShipPart(
                    ShipPartType.Gun,
                    ShipPartName.Gun1,
                    TierName.One,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Fighter] = 8
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
                [ShipPartName.Gun2] = new ShipPart(
                    ShipPartType.Gun,
                    ShipPartName.Gun2,
                    TierName.One,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Fighter] = 8
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
                [ShipPartName.Gun3] = new ShipPart(
                    ShipPartType.Gun,
                    ShipPartName.Gun3,
                    TierName.One,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Fighter] = 8
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
                [ShipPartName.Facing1] = new ShipPart(
                    ShipPartType.Facing,
                    ShipPartName.Facing1,
                    TierName.One,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Armored] = 8
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
                [ShipPartName.Facing2] = new ShipPart(
                    ShipPartType.Facing,
                    ShipPartName.Facing2,
                    TierName.One,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Armored] = 8
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
                [ShipPartName.Facing3] = new ShipPart(
                    ShipPartType.Facing,
                    ShipPartName.Facing3,
                    TierName.One,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Armored] = 8
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
                [ShipPartName.Rector1] = new ShipPart(
                    ShipPartType.Reactor,
                    ShipPartName.Rector1,
                    TierName.One,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Technodroid] = 8
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
                [ShipPartName.Rector2] = new ShipPart(
                    ShipPartType.Reactor,
                    ShipPartName.Rector1,
                    TierName.One,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Technodroid] = 8
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
                [ShipPartName.Rector3] = new ShipPart(
                    ShipPartType.Reactor,
                    ShipPartName.Rector1,
                    TierName.One,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Technodroid] = 8
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
            };
    }
}