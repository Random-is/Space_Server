using System.Collections.Generic;
using Space_Server.game;

namespace Space_Server.model {
    public class GamePlayer {
        public GamePlayer() {
        }

        public GamePlayer(string nickname) {
            Nickname = nickname;
        }

        public string Nickname { get; set; }
        public int Hp { get; set; }
        public int Money { get; set; }
        public int Xp { get; set; }
        public int Level { get; set; }
        public List<SpaceShip> SpaceShips { get; set; }
        public List<ShipComponent> BoughtComponents { get; set; }
        public Arena Arena { get; set; }

        public ShipComponent[] Shop { get; set; }

        public void Set(
            int hp,
            int money,
            int lvlUpCost,
            int level,
            List<SpaceShip> spaceShips,
            List<ShipComponent> boughtComponents,
            Arena arena,
            ShipComponent[] shop
        ) {
            Hp = hp;
            Money = money;
            Xp = lvlUpCost;
            Level = level;
            SpaceShips = spaceShips;
            BoughtComponents = boughtComponents;
            Arena = arena;
            Shop = shop;
        }

        public void Reset() {
            Set(100, 4, 6, 1, new List<SpaceShip>(), new List<ShipComponent>(), new Arena(), new ShipComponent[5]);
        }
    }
}