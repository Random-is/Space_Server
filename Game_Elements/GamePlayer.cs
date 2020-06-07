using System;
using System.Collections.Generic;
using System.Linq;
using Game_Components.arena;
using Game_Components.ship;
using Game_Components.ship.ship_part;
using Game_Components.utility;

namespace Game_Components {
    public class GamePlayer {
        public string Nickname { get; set; }
        public int Hp { get; set; }
        public int Money { get; set; }
        public int Xp { get; set; }
        public int Level { get; set; }
        public List<Ship> Ships { get; set; }
        public ShipPart[] Bag { get; set; }
        public PersonArena PersonArena { get; set; }
        public ShipPart[] Shop { get; set; }

        public void Set(
            int hp,
            int money,
            int xp,
            int level,
            List<Ship> spaceShips,
            ShipPart[] boughtComponents,
            PersonArena arena,
            ShipPart[] shop
        ) {
            Hp = hp;
            Money = money;
            Xp = xp;
            Level = level;
            Ships = spaceShips;
            Bag = boughtComponents;
            PersonArena = arena;
            Shop = shop;
        }

        public void Reset() {
            Set(100, 4, 0, 1, new List<Ship>(),
                new ShipPart[8], new PersonArena(), new ShipPart[5]
            );
        }

        public IntVector2 AddShip(ShipHullName shipHullName) {
            var newShip = new Ship(ShipHullInfo.All[shipHullName]);
            Ships.Add(newShip);
            var coordinates = PersonArena.AddShip(newShip);
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

        public bool IsNewLvl() => Xp == 0;

        public void ShipReposition(int shipIndex, int newY, int newX) {
            var ship = Ships[shipIndex];
            PersonArena.MoveShip(ship, newY, newX);
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

        public void AddBagComponent(ShipPart shipPart) {
            //todo MAKE T2 Guns
            var bagFreeSpaceIndex = Array.IndexOf(Bag, null);
            Bag[bagFreeSpaceIndex] = shipPart;
        }
    }
}