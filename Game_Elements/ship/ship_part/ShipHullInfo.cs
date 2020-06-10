using System.Collections.Generic;

namespace Game_Elements.ship.ship_part {
    public static class ShipHullInfo {
        public static readonly Dictionary<ShipHullName, ShipHull> All = new Dictionary<ShipHullName, ShipHull> {
            [ShipHullName.Armored] = new ShipHull(
                ShipHullName.Armored,
                new Dictionary<ShipParameterName, float> {
                    
                }),
            [ShipHullName.Bomber] = new ShipHull(
                ShipHullName.Bomber,
                new Dictionary<ShipParameterName, float> {
                    
                }),
            [ShipHullName.Fighter] = new ShipHull(
                ShipHullName.Fighter,
                new Dictionary<ShipParameterName, float> {
                    
                }),
            [ShipHullName.Saboteur] = new ShipHull(
                ShipHullName.Saboteur,
                new Dictionary<ShipParameterName, float> {
                    
                }),
            [ShipHullName.Technodroid] = new ShipHull(
                ShipHullName.Technodroid,
                new Dictionary<ShipParameterName, float> {
                    
                })
        };
    }
}