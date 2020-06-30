using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Game_Elements.arena;
using Game_Elements.ship;
using Game_Elements.ship.ship_part;
using Game_Elements.utility;

namespace Game_Elements.fight {
    public class FightShip {
        public const float ShipRadius = 7.5f;
        public const float MinDistanceToShip = ShipRadius * 1.5f;
        public Ship Ship { get; }
        public GamePlayer Player { get; }
        public bool Alive => Hp > 0;
        public int BusyTicksSpell { get; set; }
        public int BusyTicksAttack { get; set; }
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
        
        public FloatVector2 Position { get; set; }
        public FightArena Arena { get; }

        public float X {
            get => Position.X;
            set => Position = new FloatVector2(value, Position.Y);
        }

        public float Y {
            get => Position.Y; 
            set => Position = new FloatVector2(Position.X, value);
        }
        public float RotateAngle { get; set; }

        public FightShip(Ship ship, FloatVector2 position, FightArena arena, GamePlayer player) {
            Ship = ship;
            Player = player;
            BusyTicksSpell = 0;
            Energy = 0;
            Target = null;
            AfterBusySpells = new List<ShipPartSpell>();
            BusyTicksAttack = -1;
            Position = position;
            Arena = arena;
            RotateAngle = position.Y < FightArena.Height / 2f ? 180 : 0;
            RotateSpeed = 80;
            MoveSpeed = 10;
            AttackRange = 20;
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

        public void CalcParams(Ship ship) {
            var shipParams = ship.GetParameters();
            shipParams.AddNotExistEnumKeysToDictionary();
        }

        public void TakePhysicalDamage(int damage) {
            Hp -= damage;
        }

        public void TakeMagicDamage(int damage) {
            Hp -= damage;
        }

        public bool IsLookAt(FloatVector2 target) {
            return Math.Abs(CalcAngle(target)) < float.Epsilon;
        }

        public List<FloatVector2> FindPath(FloatVector2 target) {
            var path = new List<FloatVector2>();
            path.Add(target);
            // var pathCompleted = false;
            // var currentPosition = Position;
            // while (!pathCompleted) {
            //     
            // }
            return path;
        }

        public float CalcAngle(FloatVector2 target) {
            var targetShipPosition = MathEx.RotatePoint(X, Y, target.X, target.Y, MathEx.DegToRad(RotateAngle));
            var a1 = -1;
            var b1 = 0;
            var a2 = targetShipPosition.Y - Y;
            var b2 = X - targetShipPosition.X;
            var angleRad = Math.Acos(
                (a1 * a2 + b1 * b2) /
                (Math.Sqrt(Math.Pow(a1, 2) + Math.Pow(b1, 2)) * Math.Sqrt(Math.Pow(a2, 2) + Math.Pow(b2, 2)))
            );
            var angleDegree = MathEx.RadToDeg((float) angleRad);
            angleDegree = targetShipPosition.X <= X ? angleDegree : -angleDegree;
            angleDegree = Math.Abs(angleDegree) > 0.1 ? angleDegree : 0;
            return angleDegree;
        }

        public float CalcDistance(FightShip targetShip) {
            return FloatVector2.Distance(Position, targetShip.Position);
        }

        public void RotateTo(FloatVector2 target, in int tickRate) {
            var angleToTarget = CalcAngle(target);
            var tickRotate = RotateSpeed / tickRate;
            RotateAngle += Math.Abs(angleToTarget) > tickRotate ? Math.Sign(angleToTarget) * tickRotate : angleToTarget;
        }

        public void MoveTo(FloatVector2 target, in int tickRate) {
            var newPosition = FloatVector2.MoveTowards(
                Position,
                target,
                MoveSpeed / tickRate
            );
            foreach (var fightShip in Arena.FightShips.Where(ship => ship != this)) {
                var distanceToShip = FloatVector2.Distance(newPosition, fightShip.Position);
                if (distanceToShip < MinDistanceToShip && FloatVector2.Distance(Position, target) > FloatVector2.Distance(fightShip.Position, target)) {
                    newPosition = FloatVector2.MoveTowards(
                        newPosition,
                        Position,
                        MinDistanceToShip - distanceToShip
                    );
                    var newTarget = MathEx.RotatePoint(
                        newPosition.X, 
                        newPosition.Y, 
                        target.X, 
                        target.Y, 
                        MathEx.DegToRad(
                            Math.Sign(CalcAngle(fightShip.Position)) * 90)
                        );
                    newPosition = FloatVector2.MoveTowards(
                        newPosition,
                        newTarget,
                        MinDistanceToShip - distanceToShip
                    );
                }
            }
            Position = newPosition;
        }
    }
}