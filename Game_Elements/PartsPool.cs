using System;
using System.Collections.Generic;
using System.Linq;
using Game_Elements.ship.ship_part;

namespace Game_Elements {
    public class PartsPool<T> : Dictionary<TierName, T> where T : List<ShipPart>, new() {
        public void Generate(IEnumerable<ShipPart> components, Random random) {
            Clear();
            foreach (TierName tier in Enum.GetValues(typeof(TierName))) {
                Add(tier, new T());
            }
            foreach (var component in components) {
                for (var i = 0; i < TierInfo.Get(component.TierName).Count; i++) {
                    this[component.TierName].Insert(
                        random.Next(this[component.TierName].Count + 1),
                        component
                    );
                }
            }
        }

        public TierName[] GetRollTiers(GamePlayer player, Random random) {
            var shopLength = player.Shop.Length;
            var resultTiers = new TierName[shopLength];
            var tierChances = TierInfo.GetChances(player.Level);
            for (var i = 0; i < shopLength; i++) {
                var randomNum = random.Next(1, 101);
                var temp = 0;
                for (var j = 0; j < tierChances.Length; j++) {
                    var tierChance = tierChances[j];
                    if (randomNum <= tierChance + temp) {
                        while (
                            this[(TierName) j].Count -
                            resultTiers.Count(item => (int) item == j) == 0
                        ) {
                            j--;
                        }
                        resultTiers[i] = (TierName) j;
                        break;
                    }
                    temp += tierChance;
                }
            }
            return resultTiers;
        }

        public override string ToString() {
            var result = "";
            foreach (TierName tier in Enum.GetValues(typeof(TierName))) {
                result += $"---- Tier {tier} ----\n";
                foreach (var component in this[tier])
                    result += $"{component}\n";
            }
            return result.Remove(result.Length - 1);
        }
    }
}