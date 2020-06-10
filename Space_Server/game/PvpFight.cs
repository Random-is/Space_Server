using Game_Elements;
using Game_Elements.arena;
using Game_Elements.ship;
using Space_Server.server;

namespace Space_Server.game {
    public class PvpFight {
        public NetworkClient FirstPlayer { get; }
        public NetworkClient SecondPlayer { get; }
        public PvpArena PvpArena { get; }

        public PvpFight(NetworkClient firstPlayer, NetworkClient secondPlayer) {
            FirstPlayer = firstPlayer;
            SecondPlayer = secondPlayer;
            PvpArena = new PvpArena();
        }

        public override string ToString() {
            return $"{FirstPlayer.GamePlayer.Nickname} vs {SecondPlayer.GamePlayer.Nickname}";
        }
    }
}