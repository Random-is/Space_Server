using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using Game_Elements.ship;
using Game_Elements.utility;
using Game_Elements.arena;
using Game_Elements.ship.ship_part;

namespace Game_Elements.fight {
    public struct FightResult {
        public GamePlayer Winner;
        public GamePlayer Loser;
        public bool Tie;
        public int Damage;
    }

    public static class Fight {
        public static List<FightShip> GenerateFightShips(GamePlayer player, PvpArena arena) {
            var fightShips = player.Ships.Select(
                ship => new FightShip(ship, arena.Arena.CoordinatesOf(ship), player)
            ).ToList();
            return fightShips;
        }

        public static FightResult CalcWinner(
            GamePlayer mainPlayer,
            GamePlayer opponentPlayer,
            PvpArena arena,
            int fightDurationSeconds,
            Random random,
            Action<FightShip, List<FightShip>, int> moveAction = null,
            Action<FightShip, List<FightShip>, int> rotateAction = null,
            Action<FightShip, List<FightShip>, int> attackAction = null,
            Action<FightShip, ShipPartSpell, List<FightShip>, int> spellStartCastAction = null
        ) {
            const int tickRate = 30;
            var mainPlayerShips = GenerateFightShips(mainPlayer, arena);
            var opponentPlayerShips = GenerateFightShips(opponentPlayer, arena);
            var fightShips = mainPlayerShips.Concat(opponentPlayerShips).ToList();
            for (var i = 0; i < fightDurationSeconds * tickRate; i++) {
                Console.WriteLine($"\tTick {i}");
                for (var j = 0; j < fightShips.Count; j++) {
                    var currentShip = fightShips[j];
                    if (currentShip.BusyTicksAA == -1) {
                        if (currentShip.AfterBusySpells.Count == 0) {
                            if (currentShip.Target == null || !currentShip.Target.Alive) { //Поиск Цели
                                Console.WriteLine("Finding Target");
                                currentShip.BusyTicksSpell = 0;
                                currentShip.AfterBusySpells.Clear();
                                var minDistance = float.MaxValue;
                                FightShip minDistanceTargetShip = null;
                                foreach (var targetShip in fightShips.Where(ship => ship.Player != currentShip.Player)
                                ) {
                                    var distance = currentShip.CalcDistance(targetShip);
                                    if (distance < minDistance) {
                                        minDistance = distance;
                                        minDistanceTargetShip = targetShip;
                                    }
                                }
                                currentShip.Target = minDistanceTargetShip;
                                continue;
                            }
                            if (MathF.Abs(currentShip.CalcAngle(currentShip.Target)) > 0.1) {
                                // var s = currentShip.CalcAngle(currentShip.Target) != 0;
                                currentShip.RotateToTarget(tickRate);
                                rotateAction?.Invoke(currentShip, fightShips, tickRate);
                                if (!(MathF.Abs(currentShip.CalcAngle(currentShip.Target)) > 0.1)) {
                                    if (currentShip.CalcDistance(currentShip.Target) > currentShip.AttackRange) {
                                        Console.WriteLine($"Moving {currentShip.CalcDistance(currentShip.Target)}");
                                        currentShip.MoveToTarget(tickRate);
                                        moveAction?.Invoke(currentShip, fightShips, tickRate);
                                        continue;
                                    }
                                }
                                Console.WriteLine($"Rotating {currentShip.CalcAngle(currentShip.Target)}");
                                continue;
                            }
                            if (currentShip.CalcDistance(currentShip.Target) > currentShip.AttackRange) {
                                Console.WriteLine($"Moving {currentShip.CalcDistance(currentShip.Target)}");
                                currentShip.MoveToTarget(tickRate);
                                moveAction?.Invoke(currentShip, fightShips, tickRate);
                                continue;
                            }
                        } else if (currentShip.BusyTicksSpell == 0) { //Использование способности
                            Console.WriteLine("Spell Using Complete");
                            var spellList = currentShip.AfterBusySpells;
                            if (spellList.Count > 0) {
                                spellList[currentShip.ActiveSpellIndex].Spell(fightShips, currentShip, random);
                                spellList.RemoveAt(currentShip.ActiveSpellIndex);
                                if (spellList.Count > 0) {
                                    Console.WriteLine("Spell Using Start");
                                    var randomSpellIndex = random.Next(spellList.Count);
                                    var randomSpell = spellList[randomSpellIndex];
                                    currentShip.BusyTicksSpell = (int) (randomSpell.SpellCastSeconds * tickRate);
                                    currentShip.ActiveSpellIndex = randomSpellIndex;
                                    spellStartCastAction?.Invoke(currentShip, spellList[randomSpellIndex], fightShips,
                                        tickRate);
                                }
                                continue;
                            }
                            if (currentShip.Energy >= currentShip.MaxEnergy) {
                                Console.WriteLine("Spell Using Start Max Energy");
                                currentShip.Energy = 0;
                                foreach (var shipPart in currentShip.Ship.Parts.Values) {
                                    var spell = shipPart?.Spell;
                                    if (spell != null) {
                                        spellList.Add(spell);
                                    }
                                }
                                var randomSpellIndex = random.Next(spellList.Count);
                                var randomSpell = spellList[randomSpellIndex];
                                currentShip.BusyTicksSpell = (int) (randomSpell.SpellCastSeconds * tickRate);
                                currentShip.ActiveSpellIndex = randomSpellIndex;
                                spellStartCastAction?.Invoke(currentShip, spellList[randomSpellIndex], fightShips,
                                    tickRate);
                                continue;
                            }
                        } else {
                            currentShip.BusyTicksSpell--;
                            continue;
                        }
                        Console.WriteLine("AA start");
                        currentShip.BusyTicksAA = (int) (currentShip.AttackSpeed * tickRate); //Начало Автоатаки
                    } else if (currentShip.BusyTicksAA == 0) { //Завершение Автоатаки
                        Console.WriteLine("AA complete");
                        currentShip.Energy += currentShip.EnergyRegenPerAttack;
                        currentShip.Target.ToPhysicalDamage(currentShip.AttackDamage);
                        if (currentShip.Target.Alive) {
                            fightShips.Remove(currentShip.Target);
                        }
                        currentShip.BusyTicksAA = -1;
                        attackAction?.Invoke(currentShip, fightShips, tickRate);
                    } else {
                        currentShip.BusyTicksAA--;
                    }
                }
            }
            moveAction?.Invoke(mainPlayerShips[0], fightShips, tickRate);
            return new FightResult {
                Winner = mainPlayer,
                Loser = opponentPlayer,
                Tie = false,
                Damage = random.Next(-20, -5)
            };
        }
    }
}