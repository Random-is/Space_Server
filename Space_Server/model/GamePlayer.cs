using System.Collections.Generic;

namespace Space_Server.model {
    public class GamePlayer {
        public string Nickname { get; set; }
        public int Hp { get; set; }
        public int Money { get; set; }
        public int Xp { get; set; }
        public int Level { get; set; }
        public List<SpaceShip> SpaceShips { get; set; }
        public List<Component> BoughtComponents { get; set; }

        public GamePlayer() {
        }

        public GamePlayer(string nickname) {
            Nickname = nickname;
        }

        public void Set(int hp, int money, int xp, int level, List<SpaceShip> spaceShips,
            List<Component> boughtComponents) {
            Hp = hp;
            Money = money;
            Xp = xp;
            Level = level;
            SpaceShips = spaceShips;
            BoughtComponents = boughtComponents;
        }
    }
}