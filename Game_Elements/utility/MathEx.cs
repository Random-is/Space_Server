using System;
using System.Numerics;

namespace Game_Elements.utility {
    public static class MathEx {
        public static FloatVector2 RotatePoint(float xc, float yc, float x, float y, float angleRad) {
            var x1 = (x - xc) * Math.Cos(angleRad) - (y - yc) * Math.Sin(angleRad) + xc;
            var y1 = (x - xc) * Math.Sin(angleRad) + (y - yc) * Math.Cos(angleRad) + yc;
            return new FloatVector2 {X = (float) x1, Y = (float) y1};
        }

        public static float RadToDeg(float radAngle) {
            return (float) (radAngle * (180 / Math.PI));
        }
        
        public static float DegToRad(float degAngle) {
            return (float) (degAngle * (Math.PI / 180));
        }
    }
}