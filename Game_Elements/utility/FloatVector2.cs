using System;
using System.Numerics;

namespace Game_Elements.utility {
    public struct FloatVector2 {
        public float X;
        public float Y;

        public FloatVector2(float x, float y) {
            X = x;
            Y = y;
        }

        public static float Distance(FloatVector2 value1, FloatVector2 value2) {
            var dx = value1.X - value2.X;
            var dy = value1.Y - value2.Y;

            var ls = dx * dx + dy * dy;

            return (float) Math.Sqrt(ls);
        }

        public static FloatVector2 MoveTowards(
            FloatVector2 current,
            FloatVector2 target,
            float maxDistanceDelta
        ) {
            var num1 = target.X - current.X;
            var num2 = target.Y - current.Y;
            var num3 = num1 * num1 + num2 * num2;
            if (Math.Abs(num3) < Single.Epsilon ||
                maxDistanceDelta >= 0.0 && num3 <= maxDistanceDelta * maxDistanceDelta)
                return target;
            var num4 = Math.Sqrt(num3);
            return new FloatVector2((float) (current.X + num1 / num4 * maxDistanceDelta), (float) (current.Y + num2 / num4 * maxDistanceDelta));
        }
    }
}