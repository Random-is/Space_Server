using System.Collections.Generic;
using Space_Server.game.ship_components;

namespace Space_Server.game {
    public class GamePlayer {
        public string Nickname { get; set; }
        public int Hp { get; set; }
        public int Money { get; set; }
        public int Xp { get; set; }
        public int Level { get; set; }
        public List<SpaceShip> SpaceShips { get; set; }
        public ShipComponent[] Bag { get; set; }
        public PersonArena PersonArena { get; set; }
        public ShipComponent[] Shop { get; set; }
        
        public void Set(
            int hp,
            int money,
            int xp,
            int level,
            List<SpaceShip> spaceShips,
            ShipComponent[] boughtComponents,
            PersonArena arena,
            ShipComponent[] shop
        ) {
            Hp = hp;
            Money = money;
            Xp = xp;
            Level = level;
            SpaceShips = spaceShips;
            Bag = boughtComponents;
            PersonArena = arena;
            Shop = shop;
        }

        public void Reset() {
            Set(100, 4, 0, 1, new List<SpaceShip>(), new ShipComponent[8], new PersonArena(), new ShipComponent[5]);
        }
    }
}