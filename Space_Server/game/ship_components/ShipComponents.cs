using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Mail;

namespace Space_Server.game.ship_components {
    public enum ShipComponentType {
        Empty,
        Gun1,
        Gun2,
        Gun3,
        Gun4,
        Gun5
    }

    public static class ShipComponentInfo {
        private static readonly ImmutableDictionary<ShipComponentType, ComponentBase> Components =
            new Dictionary<ShipComponentType, ComponentBase> {
                [ShipComponentType.Gun1] = new Gun(
                    "Gun1",
                    Tier.One,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 1
                    },
                    new ComponentSpell(
                        (arena, ship, random) => { },
                        1)),
                [ShipComponentType.Gun2] = new Gun(
                    "Gun2",
                    Tier.Two,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 2
                    },
                    new ComponentSpell(
                        (arena, ship, random) => { },
                        1)),
                [ShipComponentType.Gun3] = new Gun(
                    "Gun3",
                    Tier.Three,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 3
                    },
                    new ComponentSpell(
                        (arena, ship, random) => { },
                        1)),
                [ShipComponentType.Gun4] = new Gun(
                    "Gun4",
                    Tier.Four,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 4
                    },
                    new ComponentSpell(
                        (arena, ship, random) => { },
                        1)),
                [ShipComponentType.Gun5] = new Gun(
                    "Gun5",
                    Tier.Five,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 5
                    },
                    new ComponentSpell(
                        (arena, ship, random) => { },
                        1))
            }.ToImmutableDictionary();

        public static ComponentBase Get(ShipComponentType type) {
            return Components[type];
        }
    }
}