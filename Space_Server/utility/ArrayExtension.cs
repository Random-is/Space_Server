using System.Numerics;

namespace Space_Server.utility {
    public static class ArrayExtension {
        public static IntVector2 CoordinatesOf<T>(this T[,] matrix, T value) {
            var w = matrix.GetLength(0); // width
            var h = matrix.GetLength(1); // height
            for (var x = 0; x < w; x++) {
                for (var y = 0; y < h; y++) {
                    if (matrix[x, y] != null) {
                        if (matrix[x, y].Equals(value))
                            return new IntVector2 {
                                X = x,
                                Y = y
                            };
                    }
                }
            }
            return new IntVector2 {
                X = -1,
                Y = -1
            };
        }
    }
}