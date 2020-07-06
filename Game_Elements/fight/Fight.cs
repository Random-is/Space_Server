using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        public static FightResult CalcWinner(
            FightArena arena,
            int fightDurationSeconds,
            Random random,
            Action<FightShip, FightArena, int> moveAction = null,
            Action<FightShip, FightArena, int> rotateAction = null,
            Action<FightShip, FightArena, int> attackAction = null,
            Action<FightShip, ShipPartSpell, FightArena, int> spellStartCastAction = null,
            Action<FightArena, int> afterTickAction = null
        ) {
            const int tickRate = 30;
            var fightShips = arena.FightShips;
            for (var i = 0; i < fightDurationSeconds * tickRate; i++) {
                // console.WriteLine($"\tTick {i}");
                for (var j = 0; j < fightShips.Count; j++) {
                    var currentShip = fightShips[j];
                    // console.Write($"Ship {(int) currentShip.Ship.Hull.Name} target {(currentShip.Target == null ? "Null" : ((int) currentShip.Target.Ship.Hull.Name).ToString())}: ");
                    if (currentShip.BusyTicksAttack == -1) {
                        if (currentShip.AfterBusySpells.Count == 0) {
                            if (currentShip.Target == null || !currentShip.Target.Alive) { //Поиск Цели
                                // console.WriteLine("Finding Target");
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
                            var path = currentShip.FindPath(currentShip.Target.Position);
                            if (!currentShip.IsLookAt(path[0])) {
                                // console.WriteLine($"Rotating {currentShip.CalcAngle(currentShip.Target.Position)}");
                                currentShip.RotateTo(path[0], tickRate);
                                rotateAction?.Invoke(currentShip, arena, tickRate);
                                if (currentShip.IsLookAt(path[0])) {
                                    // console.WriteLine($"Moving {currentShip.CalcDistance(currentShip.Target)}");
                                    currentShip.MoveTo(path[0], tickRate);
                                    moveAction?.Invoke(currentShip, arena, tickRate);
                                }
                                continue;
                            }
                            if (currentShip.CalcDistance(currentShip.Target) > currentShip.AttackRange) {
                                // console.WriteLine($"Moving {currentShip.CalcDistance(currentShip.Target)}");
                                currentShip.MoveTo(path[0], tickRate);
                                moveAction?.Invoke(currentShip, arena, tickRate);
                                continue;
                            }
                        } else if (currentShip.BusyTicksSpell == 0) { //Использование способности
                            // console.WriteLine("Spell Using Complete");
                            var spellList = currentShip.AfterBusySpells;
                            if (spellList.Count > 0) {
                                spellList[currentShip.ActiveSpellIndex].Spell(fightShips, currentShip, random);
                                spellList.RemoveAt(currentShip.ActiveSpellIndex);
                                if (spellList.Count > 0) {
                                    // console.WriteLine("Spell Using Start");
                                    var randomSpellIndex = random.Next(spellList.Count);
                                    var randomSpell = spellList[randomSpellIndex];
                                    currentShip.BusyTicksSpell = (int) (randomSpell.SpellCastSeconds * tickRate);
                                    currentShip.ActiveSpellIndex = randomSpellIndex;
                                    spellStartCastAction?.Invoke(currentShip, spellList[randomSpellIndex], arena,
                                        tickRate);
                                }
                                continue;
                            }
                            if (currentShip.Energy >= currentShip.MaxEnergy) {
                                // console.WriteLine("Spell Using Start Max Energy");
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
                                spellStartCastAction?.Invoke(currentShip, spellList[randomSpellIndex], arena,
                                    tickRate);
                                continue;
                            }
                        } else {
                            currentShip.BusyTicksSpell--;
                            continue;
                        }
                        // console.WriteLine("AA start");
                        currentShip.BusyTicksAttack = (int) (1 / currentShip.AttackSpeed * tickRate); //Начало Автоатаки
                    } else if (currentShip.BusyTicksAttack == 0) { //Завершение Автоатаки
                        // console.WriteLine("AA complete");
                        currentShip.Energy += currentShip.EnergyRegenPerAttack;
                        currentShip.Target.TakePhysicalDamage(currentShip.AttackDamage);
                        if (!currentShip.Target.Alive) {
                            fightShips.Remove(currentShip.Target);
                        }
                        currentShip.BusyTicksAttack = -1;
                        attackAction?.Invoke(currentShip, arena, tickRate);
                    } else {
                        // console.WriteLine($"AA wating {currentShip.BusyTicksAA}");
                        currentShip.BusyTicksAttack--;
                    }
                }
                afterTickAction?.Invoke(arena, tickRate);
            }
            var mainPlayerShipsCount = fightShips.Count(ship => ship.Player == arena.MainPlayer);
            var opponentShipsCount = fightShips.Count(ship => ship.Player == arena.OpponentPlayer);
            var winner = mainPlayerShipsCount > opponentShipsCount ? arena.MainPlayer : arena.OpponentPlayer;
            var looser = winner == arena.MainPlayer ? arena.OpponentPlayer : arena.MainPlayer;
            var tie = mainPlayerShipsCount == opponentShipsCount;
            var damage = mainPlayerShipsCount > opponentShipsCount ? mainPlayerShipsCount : opponentShipsCount;
            damage *= 4;
            return new FightResult {
                Winner = winner,
                Loser = looser,
                Tie = tie,
                Damage = damage
            };
        }
    }
}