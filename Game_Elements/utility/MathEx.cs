using System;
using System.Numerics;

namespace Game_Elements.utility {
    public static class MathEx {
        public static Vector2 RotatePoint(float xc, float yc, float x, float y, float angleRad) {
            var x1 = (x - xc) * MathF.Cos(angleRad) - (y - yc) * MathF.Sin(angleRad) + xc;
            var y1 = (x - xc) * MathF.Sin(angleRad) + (y - yc) * MathF.Cos(angleRad) + yc;
            return new Vector2 {X = x1, Y = y1};
        }

        public static float RadToDeg(float radAngle) {
            return radAngle * (180 / MathF.PI);
        }
        
        public static float DegToRad(float degAngle) {
            return degAngle * (MathF.PI / 180);
        }
    }
}