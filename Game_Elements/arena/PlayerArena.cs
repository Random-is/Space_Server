using System;
using Game_Elements.ship;
using Game_Elements.utility;

namespace Game_Elements.arena {
    public class PlayerArena {
        public const int YCount = 3;
        public const int XCount = 6;
        public Ship[,] Arena { get; } = new Ship[YCount, XCount];

        public IntVector2 AddShipToFreePosition(Ship ship) {
            var freeSpaceCoordinates = GetFreeSpace();
            Arena[freeSpaceCoordinates.Y, freeSpaceCoordinates.X] = ship;
            return freeSpaceCoordinates;
        }
        
        public IntVector2 AddShipToPosition(Ship ship, IntVector2 position) {
            var freeSpaceCoordinates = position;
            Arena[freeSpaceCoordinates.Y, freeSpaceCoordinates.X] = ship;
            return freeSpaceCoordinates;
        }

        public Ship ShipReposition(Ship ship, int newY, int newX) {
            var oldCoordinates = Arena.CoordinatesOf(ship);
            Console.WriteLine(oldCoordinates);
            Console.WriteLine(Arena[newY, newX]);
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