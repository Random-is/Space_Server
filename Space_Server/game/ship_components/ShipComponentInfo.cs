using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Mail;

namespace Space_Server.game.ship_components {
    public static class ShipComponentInfo {
        public static readonly ImmutableDictionary<string, ShipComponent> Components =
            new Dictionary<string, ShipComponent> {
                [ShipComponentName.Gun1] = new ShipComponent(
                    ShipComponentType.Gun,
                    ShipComponentName.Gun1,
                    Tier.One,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 1
                    },
                    new ComponentSpell(
                        (arena, ship, random) => { },
                        1)),
                [ShipComponentName.Gun2] = new ShipComponent(
                    ShipComponentType.Gun,
                    ShipComponentName.Gun2,
                    Tier.Two,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 2
                    },
                    new ComponentSpell(
                        (arena, ship, random) => { },
                        1)),
                [ShipComponentName.Gun3] = new ShipComponent(
                    ShipComponentType.Gun,
                    ShipComponentName.Gun3,
                    Tier.Three,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 3
                    },
                    new ComponentSpell(
                        (arena, ship, random) => { },
                        1)),
                [ShipComponentName.Gun4] = new ShipComponent(
                    ShipComponentType.Gun,
                    ShipComponentName.Gun4,
                    Tier.Four,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 4
                    },
                    new ComponentSpell(
                        (arena, ship, random) => { },
                        1)),
                [ShipComponentName.Gun5] = new ShipComponent(
                    ShipComponentType.Gun,
                    ShipComponentName.Gun5,
                    Tier.Five,
                    new Dictionary<ShipClassType, int> {
                        [ShipClassType.Istrebitel] = 5
                    },
                    new ComponentSpell(
                        (arena, ship, random) => { },
                        1))
            }.ToImmutableDictionary();
    }
}