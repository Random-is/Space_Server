using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Game_Elements.fight;
using Game_Elements.utility;

namespace Game_Elements.arena {
    public class FightArena {
        public const int YCount = PlayerArena.YCount + 1;
        public const int XCount = PlayerArena.XCount;
        public const int CellHeight = 10;
        public const int CellWidth = 10;
        public const int Height = YCount * CellHeight;
        public const int Width = XCount * CellWidth;

        public GamePlayer MainPlayer { get; }
        public GamePlayer OpponentPlayer { get; }
        public List<FightShip> FightShips { get; }

        public FightArena(GamePlayer mainPlayer, GamePlayer opponentPlayer) {
            MainPlayer = mainPlayer;
            OpponentPlayer = opponentPlayer;
            FightShips = GenerateFightShips(mainPlayer, opponentPlayer);
        }

        public List<FightShip> GenerateFightShips(GamePlayer mainPlayer, GamePlayer opponentPlayer) {
            var mainPlayerShips = GeneratePlayerFightShips(
                mainPlayer,
                (y, x) =>
                    new Vector2(
                        x * CellWidth + CellWidth / 2,
                        (YCount - PlayerArena.YCount + y) * CellHeight + CellHeight / 2
                    )
            );
            var opponentPlayerShips = GeneratePlayerFightShips(
                opponentPlayer,
                (y, x) =>
                    new Vector2(
                        (PlayerArena.XCount - 1 - x) * CellWidth + CellWidth / 2,
                        (PlayerArena.YCount - 1 - y) * CellHeight + CellHeight / 2
                    )
            );
            return mainPlayerShips.Concat(opponentPlayerShips).ToList();
        }

        private List<FightShip> GeneratePlayerFightShips(
            GamePlayer player,
            Func<int, int, Vector2> positionConversion
        ) {
            var fightShips = new List<FightShip>();
            var arena = player.PlayerArena.Arena;
            for (var y = 0; y < arena.GetLength(0); y++) {
                for (var x = 0; x < arena.GetLength(1); x++) {
                    if (arena[y, x] != null) {
                        fightShips.Add(
                            new FightShip(
                                arena[y, x],
                                positionConversion(y, x),
                                this,
                                player
                            )
                        );
                    }
                }
            }
            return fightShips;
        }

        public override string ToString() {
            var result = string.Join(',', FightShips.Select(ship => ship.Hp));
            for (var y = 0; y < Height; y++) {
                result += "\n";
                for (var x = 0; x < Width; x++) {
                    var foundShip = FightShips.Find(ship => Vector2.Distance(ship.Position, new Vector2(x, y)) < FightShip.ShipRadius / 2);
                    // var foundShip = FightShips.Find(ship => (int) MathF.Round(ship.Position.X) == x && (int) MathF.Round(ship.Position.Y) == y);
                    if (foundShip != null) {
                        // result += Math.Abs(foundShip.RotateAngle) < 90 ? "↑" : "↓";
                        result += ((int) foundShip.Ship.Hull.Name).ToString();
                    } else {
                        result += ".";
                    }
                }
            }
            return result;
        }
    }
}