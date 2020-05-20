using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Mail;

namespace Space_Server.game.ship_components {
    
    public enum ShipComponentType {
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
                    (arena, ship) => { }),
                [ShipComponentType.Gun2] = new Gun(
                    "Gun2",
                    Tier.Two,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 2
                    },
                    (arena, ship) => { }),
                [ShipComponentType.Gun3] = new Gun(
                    "Gun3",
                    Tier.Three,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 3
                    },
                    (arena, ship) => { }),
                [ShipComponentType.Gun4] = new Gun(
                    "Gun4",
                    Tier.Four,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 4
                    },
                    (arena, ship) => { }),
                [ShipComponentType.Gun5] = new Gun(
                    "Gun5",
                    Tier.Five,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 5
                    },
                    (arena, ship) => { })
            }.ToImmutableDictionary();

        public static ComponentBase Get(ShipComponentType type) {
            return Components[type];
        }
    }

    public static class ShipComponents {
        public static readonly List<ComponentBase> All = new List<ComponentBase> {
            new Gun(
                "Gun1",
                Tier.One,
                new Dictionary<ShipClassType, int> {
                    [ShipClassType.Istrebitel] = 1
                },
                (arena, ship) => { }),
            new Gun(
                "Gun2",
                Tier.Two,
                new Dictionary<ShipClassType, int> {
                    [ShipClassType.Istrebitel] = 2
                },
                (arena, ship) => { }),
            new Gun(
                "Gun3",
                Tier.Three,
                new Dictionary<ShipClassType, int> {
                    [ShipClassType.Istrebitel] = 3
                },
                (arena, ship) => { }),
            new Gun(
                "Gun4",
                Tier.Four,
                new Dictionary<ShipClassType, int> {
                    [ShipClassType.Istrebitel] = 4
                },
                (arena, ship) => { }),
            new Gun(
                "Gun5",
                Tier.Five,
                new Dictionary<ShipClassType, int> {
                    [ShipClassType.Istrebitel] = 5
                },
                (arena, ship) => { })
        };
    }
}