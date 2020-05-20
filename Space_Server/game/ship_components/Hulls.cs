using System.Collections.Generic;
using System.Collections.Immutable;

namespace Space_Server.game.ship_components {
    public class Hull {
        public string Name { get; }
        public Hull(string name) {
            Name = name;
        }
    }

    public enum HullType {
        Istrebitel,
        Diversant,
        Bronenosec,
        Technodroid,
        Bombardirovshik
    }
    
    public static class Hulls {
        public static readonly ImmutableDictionary<HullType, Hull> All = new Dictionary<HullType, Hull> {
            [HullType.Istrebitel] = new Hull(HullType.Istrebitel.ToString()),
            [HullType.Diversant] = new Hull(HullType.Diversant.ToString()),
            [HullType.Bronenosec] = new Hull(HullType.Bronenosec.ToString()),
            [HullType.Technodroid] = new Hull(HullType.Technodroid.ToString()),
            [HullType.Bombardirovshik] = new Hull(HullType.Bombardirovshik.ToString())
        }.ToImmutableDictionary();
    }
}