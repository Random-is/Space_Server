using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Space_Server.model {
    public class TierInfo {
        public static readonly ConcurrentDictionary<int, int[]> TierChancesByLvl = new ConcurrentDictionary<int, int[]> {
            [1] = new []{100, 0, 0, 0, 0},
            [2] = new []{50, 50, 0, 0, 0},
            [3] = new []{34, 33, 33, 0, 0},
            [4] = new []{25, 25, 25, 25, 0},
            [5] = new []{35, 20, 20, 20, 5},
        };
        
        public static readonly TierInfo[] TierList = {
            new TierInfo(100, GetAllShipComponents(typeof(IFirstTier))),
            new TierInfo(8, GetAllShipComponents(typeof(ISecondTier))),
            new TierInfo(6, GetAllShipComponents(typeof(IThirdTier))),
            new TierInfo(4, GetAllShipComponents(typeof(IFourthTier))),
            new TierInfo(2, GetAllShipComponents(typeof(IFifthTier)))
        };

        private TierInfo(int instanceCount, Type[] types) {
            InstanceCount = instanceCount;
            Types = types;
        }

        public int InstanceCount { get; }
        public Type[] Types { get; }

        private static Type[] GetAllShipComponents(Type tierType) {
            return Assembly.GetExecutingAssembly().ExportedTypes.Where(
                type => type.IsSubclassOf(typeof(ShipComponent)) &&
                        type.GetInterfaces().Contains(tierType)).ToArray();
        }
    }
}