using System.Collections.Generic;

namespace Game_Components.ship.ship_part {
    public static class ShipComponentInfo {
        public static readonly Dictionary<ShipPartName, ShipPart> All =
            new Dictionary<ShipPartName, ShipPart> {
                [ShipPartName.Gun1] = new ShipPart(
                    ShipPartType.Gun,
                    ShipPartName.Gun1,
                    TierName.One,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Fighter] = 1
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
                [ShipPartName.Gun2] = new ShipPart(
                    ShipPartType.Gun,
                    ShipPartName.Gun2,
                    TierName.Two,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Fighter] = 2
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
                [ShipPartName.Gun3] = new ShipPart(
                    ShipPartType.Gun,
                    ShipPartName.Gun3,
                    TierName.Three,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Fighter] = 3
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
                [ShipPartName.Gun4] = new ShipPart(
                    ShipPartType.Gun,
                    ShipPartName.Gun4,
                    TierName.Four,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Fighter] = 4
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1)),
                [ShipPartName.Gun5] = new ShipPart(
                    ShipPartType.Gun,
                    ShipPartName.Gun5,
                    TierName.Five,
                    new Dictionary<ShipClassName, int> {
                        [ShipClassName.Fighter] = 5
                    },
                    new ShipPartSpell(
                        (fightShips, ship, random) => { },
                        1))
            };
    }
}