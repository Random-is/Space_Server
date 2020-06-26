using Game_Elements;
using Game_Elements.arena;
using Game_Elements.ship;
using Space_Server.server;

namespace Space_Server.game {
    public class PvpFight {
        public NetworkClient MainPlayer { get; }
        public NetworkClient OpponentPlayer { get; }
        public FightArena PvpArena { get; }

        public PvpFight(NetworkClient mainPlayer, NetworkClient opponentPlayer) {
            MainPlayer = mainPlayer;
            OpponentPlayer = opponentPlayer;
            PvpArena = new FightArena(mainPlayer.GamePlayer, opponentPlayer.GamePlayer);
        }

        public override string ToString() {
            return $"{MainPlayer.GamePlayer.Nickname} vs {OpponentPlayer.GamePlayer.Nickname}";
        }
    }
}