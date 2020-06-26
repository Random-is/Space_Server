using System.Collections.Generic;

namespace Game_Elements.ship.ship_part {
    public static class ShipHullInfo {
        public static readonly Dictionary<ShipHullName, ShipHull> All = new Dictionary<ShipHullName, ShipHull> {
            [ShipHullName.Armored] = new ShipHull(
                ShipHullName.Armored,
                new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 1200,
                    [ShipParameterName.Energy] = 100,
                    [ShipParameterName.AttackDamage] = 20,
                    [ShipParameterName.MagicDamage] = 10,
                    [ShipParameterName.Armor] = 60,
                    [ShipParameterName.MagicResist] = 40,
                    [ShipParameterName.AttackRange] = 2,
                    [ShipParameterName.AttackSpeed] = 0.2f
                }),
            [ShipHullName.Bomber] = new ShipHull(
                ShipHullName.Bomber,
                new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 900,
                    [ShipParameterName.Energy] = 120,
                    [ShipParameterName.AttackDamage] = 15,
                    [ShipParameterName.MagicDamage] = 50,
                    [ShipParameterName.Armor] = 20,
                    [ShipParameterName.MagicResist] = 20,
                    [ShipParameterName.AttackRange] = 3,
                    [ShipParameterName.AttackSpeed] = 0.5f
                }),
            [ShipHullName.Fighter] = new ShipHull(
                ShipHullName.Fighter,
                new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 720,
                    [ShipParameterName.Energy] = 90,
                    [ShipParameterName.AttackDamage] = 110,
                    [ShipParameterName.MagicDamage] = 10,
                    [ShipParameterName.Armor] = 20,
                    [ShipParameterName.MagicResist] = 20,
                    [ShipParameterName.AttackRange] = 3,
                    [ShipParameterName.AttackSpeed] = 0.68f
                }),
            [ShipHullName.Saboteur] = new ShipHull(
                ShipHullName.Saboteur,
                new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 700,
                    [ShipParameterName.Energy] = 120,
                    [ShipParameterName.AttackDamage] = 110,
                    [ShipParameterName.MagicDamage] = 50,
                    [ShipParameterName.Armor] = 20,
                    [ShipParameterName.MagicResist] = 20,
                    [ShipParameterName.AttackRange] = 3,
                    [ShipParameterName.AttackSpeed] = 0.5f,
                    [ShipParameterName.CritChance] = 0.25f,
                    [ShipParameterName.CritDamage] = 100,
                }),
            [ShipHullName.Technodroid] = new ShipHull(
                ShipHullName.Technodroid,
                new Dictionary<ShipParameterName, float> {
                    [ShipParameterName.Health] = 900,
                    [ShipParameterName.Energy] = 220,
                    [ShipParameterName.AttackDamage] = 30,
                    [ShipParameterName.MagicDamage] = 40,
                    [ShipParameterName.Armor] = 20,
                    [ShipParameterName.MagicResist] = 20,
                    [ShipParameterName.AttackRange] = 4,
                    [ShipParameterName.AttackSpeed] = 0.4f,
                })
        };
    }
}