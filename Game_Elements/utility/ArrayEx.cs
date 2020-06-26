using System.Collections.Generic;

namespace Game_Elements.utility {
    public static class ArrayEx {
        public static IntVector2 CoordinatesOf<T>(this T[,] array, T value) {
            for (var y = 0; y < array.GetLength(0); y++) {
                for (var x = 0; x < array.GetLength(1); x++) {
                    if (Equals(array[y, x], value))
                        return new IntVector2 {Y = y, X = x};
                }
            }
            return new IntVector2 {X = -1, Y = -1};
        }

        public static T Get<T>(this T[,] array, IntVector2 position) {
            return array[position.Y, position.X];
        }
    }
}