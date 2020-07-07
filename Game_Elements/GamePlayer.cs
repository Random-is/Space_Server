using System;
using System.Collections.Generic;
using System.Linq;
using Game_Elements.arena;
using Game_Elements.ship;
using Game_Elements.ship.ship_part;
using Game_Elements.utility;

namespace Game_Elements {
    public class GamePlayer : IGamePlayer {
        public string Nickname { get; set; }
        public int Hp { get; set; }
        public int Money { get; set; }
        public int Xp { get; set; }
        public int Lvl { get; set; }
        public List<Ship> Ships { get; set; }
        public ShipPart[] Bag { get; set; }
        public PlayerArena PlayerArena { get; set; }
        public ShipPart[] Shop { get; set; }
        
        public bool ShopLock { get; set; }

        public const int BaseXp = 2;
        public const int MaxLvl = 5;

        public void Set(
            int hp,
            int money,
            int xp,
            int level,
            List<Ship> spaceShips,
            ShipPart[] boughtComponents,
            PlayerArena arena,
            ShipPart[] shop,
            bool shopLock
        ) {
            Hp = hp;
            Money = money;
            Xp = xp;
            Lvl = level;
            Ships = spaceShips;
            Bag = boughtComponents;
            PlayerArena = arena;
            Shop = shop;
        }

        public void Reset() {
            Set(100, 4, 0, 2, new List<Ship>(),
                new ShipPart[8], new PlayerArena(), new ShipPart[5], false
            );
        }

        public void AddShipToPosition(Ship ship, IntVector2 position) {
            Ships.Add(ship);
            PlayerArena.AddShipToPosition(ship, position);
        }

        public void SetMoney(int money) {
            Money = money;
        }

        public IntVector2 AddShipToFreePosition(Ship ship) {
            Ships.Add(ship);
            var coordinates = PlayerArena.AddShipToFreePosition(ship);
            return coordinates;
        }

        public void SetHp(int hp) {
            Hp = hp;
        }

        public void ChangeMoney(int money) {
            Money += money;
        }

        public void SetXp(int xp) {
            Xp = xp;
        }

        public void ChangeHp(int hp) {
            Hp += hp;
        }

        public void AddXp(int xp) {
            if (Lvl < MaxLvl) {
                Xp += xp;
                if (Xp >= Lvl + BaseXp) {
                    Xp = 0;
                    Lvl += 1;
                }
            }
        }

        public List<int> IndexesToCreateT2(ShipPart shipPart) {
            var count = Bag.Count(part => part == shipPart);
            if (count >= 2) {
                var indexesRoRemove = new List<int> {Array.IndexOf(Bag, shipPart), Array.LastIndexOf(Bag, shipPart)};
                return indexesRoRemove;
            }
            return null;
        }

        public ShipPart AddShipPartToShipAndSell(Ship ship, int bagIndex) {
            var shipPart = Bag[bagIndex];
            if (shipPart == ship.Parts[shipPart.Type]) {
                var indexesToCreateT2 = IndexesToCreateT2(shipPart);
                if (indexesToCreateT2 != null) {
                    if (indexesToCreateT2.Count == 2) {
                        foreach (var i in indexesToCreateT2) {
                            Bag[i] = null;
                        }
                        shipPart = ShipPartInfo.T2Rank[shipPart.Name];
                        ship.ChangeComponent(shipPart);
                        return shipPart;
                    } 
                } else {
                    return null;
                }
            } else {
                Bag[bagIndex] = null;
                var oldShipPart = ship.ChangeComponent(shipPart);
                if (oldShipPart != null) {
                    ChangeMoney(TierInfo.Get(oldShipPart.TierName).Cost * (oldShipPart.Rank == 1 ? 1 : 3));
                }
            }
            return shipPart;
        }
        
        public int AddBagItem(ShipPart shipPart) {
            var indexesToCreateT2 = IndexesToCreateT2(shipPart);
            if (indexesToCreateT2 != null) {
                if (indexesToCreateT2.Count == 2) {
                    foreach (var i in indexesToCreateT2) {
                        Bag[i] = null;
                    }
                    shipPart = ShipPartInfo.T2Rank[shipPart.Name];
                } 
            }
            var bagFreeSpaceIndex = Array.IndexOf(Bag, null);
            Bag[bagFreeSpaceIndex] = shipPart;
            return bagFreeSpaceIndex;
        }

        public ShipPart BuyShipPart(int shopItemIndex) {
            var shipPart = Shop[shopItemIndex];
            ChangeMoney(-TierInfo.Get(shipPart.TierName).Cost);
            Shop[shopItemIndex] = null;
            return shipPart;
        }

        public void SetShop(ShipPart[] shop) {
            Shop = shop;
        }

        public bool IsNewLvl() => Xp == 0;

        public Ship ShipReposition(Ship ship, int newY, int newX) {
            return PlayerArena.ShipReposition(ship, newY, newX);
        }

        public bool CanBuyShipPart(int shopIndex) {
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

        public ShipPart SellBagItem(int bagIndex) {
            var shipPart = Bag[bagIndex];
            Bag[bagIndex] = null;
            ChangeMoney(TierInfo.Get(shipPart.TierName).Cost * (shipPart.Rank == 1 ? 1 : 3));
            return shipPart;
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