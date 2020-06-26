using System;
using System.Collections.Generic;
using System.Linq;
using Game_Elements.arena;
using Game_Elements.ship;
using Game_Elements.ship.ship_part;
using Game_Elements.utility;

namespace Game_Elements {
    public class GamePlayer {
        public string Nickname { get; set; }
        public int Hp { get; set; }
        public int Money { get; set; }
        public int Xp { get; set; }
        public int Level { get; set; }
        public List<Ship> Ships { get; set; }
        public ShipPart[] Bag { get; set; }
        public PlayerArena PlayerArena { get; set; }
        public ShipPart[] Shop { get; set; }

        public void Set(
            int hp,
            int money,
            int xp,
            int level,
            List<Ship> spaceShips,
            ShipPart[] boughtComponents,
            PlayerArena arena,
            ShipPart[] shop
        ) {
            Hp = hp;
            Money = money;
            Xp = xp;
            Level = level;
            Ships = spaceShips;
            Bag = boughtComponents;
            PlayerArena = arena;
            Shop = shop;
        }

        public void Reset() {
            Set(100, 4, 0, 1, new List<Ship>(),
                new ShipPart[8], new PlayerArena(), new ShipPart[5]
            );
        }

        public IntVector2 AddShip(ShipHullName shipHullName) {
            var newShip = new Ship(ShipHullInfo.All[shipHullName]);
            Ships.Add(newShip);
            var coordinates = PlayerArena.AddShip(newShip);
            return coordinates;
        }

        public void ChangeMoney(int money) {
            Money += money;
        }

        public void ChangeHp(int hp) {
            Hp += hp;
        }

        public void AddXp(int xp) {
            if (Level < 5) {
                Xp += xp;
                if (Xp >= 12) {
                    Xp = 0;
                    Level += 1;
                }
            }
        }

        public ShipPart AddShipPartToShip(Ship ship, int bagIndex) {
            var shipPart = Bag[bagIndex];
            Bag[bagIndex] = null;
            return ship.ChangeComponent(shipPart);
        }

        public bool IsNewLvl() => Xp == 0;

        public Ship ShipReposition(Ship ship, int newY, int newX) {
            return PlayerArena.ShipReposition(ship, newY, newX);
        }

        public bool CanBuyComponent(int shopIndex) {
            //todo T2 GUNS checking
            var hasShopItem = Shop[shopIndex] != null;
            if (hasShopItem) {
                var enoughMoney = Money >= TierInfo.Get(Shop[shopIndex].TierName).Cost;
                var hasBagPlace = Bag.Count(
                    component => component == null
                ) > 0;
                return enoughMoney && hasBagPlace;
            }
            return false;
        }

        public ShipPart BuyComponent(int shopItemIndex) {
            var shipPart = Shop[shopItemIndex];
            ChangeMoney(-TierInfo.Get(shipPart.TierName).Cost);
            Shop[shopItemIndex] = null;
            return shipPart;
        }

        public int AddBagComponent(ShipPart shipPart) {
            //todo MAKE T2 Guns
            var bagFreeSpaceIndex = Array.IndexOf(Bag, null);
            Bag[bagFreeSpaceIndex] = shipPart;
            return bagFreeSpaceIndex;
        }

        public ShipPart BagItemReposition(int oldItemIndex, int newItemIndex) {
            var targetItem = Bag[newItemIndex];
            Bag[newItemIndex] = Bag[oldItemIndex];
            Bag[oldItemIndex] = targetItem;
            return targetItem;
        }

        public Dictionary<ShipClassName, int> GetShipsClasses() {
            var shipsClasses = new Dictionary<ShipClassName, int>();
            foreach (var shipClass in Ships.SelectMany(ship => ship.GetActiveClasses())) {
                if (shipsClasses.ContainsKey(shipClass)) {
                    shipsClasses[shipClass] += 1;
                } else {
                    shipsClasses[shipClass] = 1;
                }
            }
            return shipsClasses;
        }
    }
}