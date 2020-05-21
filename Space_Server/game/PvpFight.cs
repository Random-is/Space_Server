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

        public float CalcDistance(SpaceShip firstShip, SpaceShip secondShip) {
            return PvpArena.CalcDistance(firstShip, secondShip);
        }
    }
}