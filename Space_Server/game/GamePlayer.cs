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
        public ComponentBase[] BoughtComponents { get; set; }
        public PersonArena PersonArena { get; set; }
        public ComponentBase[] Shop { get; set; }
        
        public void Set(
            int hp,
            int money,
            int xp,
            int level,
            List<SpaceShip> spaceShips,
            ComponentBase[] boughtComponents,
            PersonArena arena,
            ComponentBase[] shop
        ) {
            Hp = hp;
            Money = money;
            Xp = xp;
            Level = level;
            SpaceShips = spaceShips;
            BoughtComponents = boughtComponents;
            PersonArena = arena;
            Shop = shop;
        }

        public void Reset() {
            Set(100, 4, 0, 1, new List<SpaceShip>(), new ComponentBase[8], new PersonArena(), new ComponentBase[5]);
        }
    }
}