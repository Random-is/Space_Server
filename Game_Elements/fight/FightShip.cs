using System;
using System.Collections.Generic;
using System.Numerics;
using Game_Components.arena;
using Game_Components.ship;
using Game_Components.ship.ship_part;
using Game_Components.utility;

namespace Game_Components.fight {
    public class FightShip {
        public Ship Ship { get; set; }
        public GamePlayer Player { get; }
        public bool Alive { get; set; }
        public int BusyTicksSpell { get; set; }
        public int BusyTicksAA { get; set; }
        public List<ShipPartSpell> AfterBusySpells { get; set; }
        public int ActiveSpellIndex { get; set; }
        public FightShip Target { get; set; }
        public float AttackRange { get; set; }
        public float AttackSpeed { get; set; }
        public float MoveSpeed { get; set; }
        public float RotateSpeed { get; set; }
        public int Energy { get; set; }
        public int AttackDamage { get; set; }
        public int MaxEnergy { get; set; }
        public int EnergyRegenPerAttack { get; set; }
        public int Hp { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float RotateAngle { get; set; }

        public FightShip(Ship ship, IntVector2 coordinates, GamePlayer player) {
            Ship = ship;
            Player = player;
            Alive = true;
            BusyTicksSpell = 0;
            Energy = 0;
            Target = null;
            AfterBusySpells = new List<ShipPartSpell>();
            BusyTicksAA = -1;
            CalcPosition(coordinates, 5, 5);
            RotateAngle = coordinates.Y < PvpArena.YSize / 2 ? 180 : 0;
            RotateSpeed = 50;
            MoveSpeed = 15;
            AttackRange = 5;
            Hp = 15;
            AttackSpeed = 3;
            AttackDamage = 1;

            //calc All params
            //calc HP
            //calc MaxEnergy
            //calc speed/second
            //calc attack speed/second
            //calc spell damage
            //armor
            //etc
        }

        public void ToPhysicalDamage(int damage) {
            Hp -= damage;
            if (Hp <= 0) {
                Alive = false;
            }
        }

        public void ToMagicDamage(int damage) {
            Hp -= damage;
        }

        public float CalcAngle(FightShip targetShip) {
            var targetShipPosition = MathEx.RotatePoint(X, Y, targetShip.X, targetShip.Y, RotateAngle * MathF.PI / 180);
            var a1 = -1;
            var b1 = 0;
            var a2 = targetShipPosition.Y - Y;
            var b2 = X - targetShipPosition.X;
            var angleRad = MathF.Acos(
                (a1 * a2 + b1 * b2) /
                (MathF.Sqrt(MathF.Pow(a1, 2) + MathF.Pow(b1, 2)) * MathF.Sqrt(MathF.Pow(a2, 2) + MathF.Pow(b2, 2)))
            );
            var angleDegree = angleRad * (180 / MathF.PI);
            angleDegree = targetShipPosition.X <= X ? angleDegree : -angleDegree;
            return angleDegree;
        }

        public float CalcDistance(FightShip targetShip) {
            return Vector2.Distance(new Vector2(X, Y), new Vector2(targetShip.X, targetShip.Y));
        }

        public void CalcPosition(IntVector2 coordinates, int cellXSize, int cellYSize) {
            X = coordinates.X * cellXSize + cellXSize / 2;
            Y = coordinates.Y * cellYSize + cellYSize / 2;
        }

        public void RotateToTarget(in int tickRate) {
            var angleToTarget = CalcAngle(Target);
            var tickRotate = RotateSpeed / tickRate;
            // Console.WriteLine($"RTT {angleToTarget} {tickRotate} {RotateAngle}");
            RotateAngle += MathF.Abs(angleToTarget) > tickRotate ? MathF.Sign(angleToTarget) * tickRotate : angleToTarget;
        }

        public void MoveToTarget(in int tickRate) {
            var newPosition = Vector2Ex.MoveTowards(new Vector2(X, Y),
                new Vector2(Target.X, Target.Y),
                MoveSpeed / tickRate
            );
            Y = newPosition.Y;
            X = newPosition.X;
        }
    }
}