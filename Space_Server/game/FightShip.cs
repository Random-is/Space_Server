using System;
using System.Collections.Generic;
using Space_Server.game.ship_components;
using Space_Server.utility;

namespace Space_Server.game {
    public class FightShip {
        public SpaceShip Ship { get; set; }
        public bool Alive { get; set; }
        public int BusyTicksSpell { get; set; }
        public int BusyTicksAA { get; set; }
        public List<ComponentSpell> AfterBusySpells { get; set; }
        public int ActiveSpellIndex { get; set; }
        public FightShip Target { get; set; }
        public float AttackRange { get; set; }
        public float AttackSpeed { get; set; }
        public int Energy { get; set; }
        public int AttackDamage { get; set; }
        public int MaxEnergy { get; set; }
        public int EnergyRegenPerAttack { get; set; }
        public int Hp { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        
        public float Rotate { get; set; }
        
        public FightShip(SpaceShip ship) {
            Ship = ship;
            Alive = true;
            BusyTicksSpell = 0;
            Energy = 0;
            Target = null;
            AfterBusySpells = new List<ComponentSpell>();
            BusyTicksAA = -1;
            //calc All params
            //calc HP
            //calc MaxEnergy
            //calc speed/second
            //calc attack speed/second
            //calc spell damage
            //armor
            //etc
        }

        public void CalcPosition(PvpArena arena, int cellSize) {
            var coordinates = arena.Arena.CoordinatesOf(Ship);
            X = coordinates.X * cellSize + cellSize / 2;
            Y = coordinates.Y * cellSize + cellSize / 2;
            Rotate = coordinates.X < arena.Arena.GetLength(0)/2 ? 180 : 0;
        }
    }
}