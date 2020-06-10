using System;
using System.Numerics;

namespace Game_Elements.utility {
    public static class Vector2Ex {
        public static Vector2 MoveTowards(
            Vector2 current,
            Vector2 target,
            float maxDistanceDelta
        ) {
            var num1 = target.X - current.X;
            var num2 = target.Y - current.Y;
            var num3 = num1 * num1 + num2 * num2;
            if (Math.Abs(num3) < float.Epsilon ||
                maxDistanceDelta >= 0.0 && num3 <= maxDistanceDelta * maxDistanceDelta)
                return target;
            var num4 = MathF.Sqrt(num3);
            return new Vector2(current.X + num1 / num4 * maxDistanceDelta, current.Y + num2 / num4 * maxDistanceDelta);
        }
    }
}