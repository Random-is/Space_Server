using System;
using Game_Elements.ship;
using Game_Elements.utility;

namespace Game_Elements.arena {
    public class PersonArena {
        public const int YSize = 3;
        public const int XSize = 6;
        public Ship[,] Arena { get; } = new Ship[YSize, XSize];

        public IntVector2 AddShip(Ship ship) {
            var freeSpaceCoordinates = GetFreeSpace();
            Arena[freeSpaceCoordinates.Y, freeSpaceCoordinates.X] = ship;
            return freeSpaceCoordinates;
        }

        public Ship ShipReposition(Ship ship, int newY, int newX) {
            var oldCoordinates = Arena.CoordinatesOf(ship);
            var targetShip = Arena[newY, newX];
            Arena[newY, newX] = ship;
            Arena[oldCoordinates.Y, oldCoordinates.X] = targetShip;
            return targetShip;
        }

        public IntVector2 GetFreeSpace() {
            for (var y = Arena.GetLength(0) - 1; y >= 0; y--) {
                for (var x = 0; x < Arena.GetLength(1); x++) {
                    if (IsFree(y, x)) {
                        return new IntVector2 {Y = y, X = x};
                    }
                }
            }
            return new IntVector2 {Y = -1, X = -1};
        }

        public bool IsFree(int y, int x) {
            return Arena[y, x] == null;
        }
    }
}