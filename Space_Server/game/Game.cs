using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Space_Server.game.ship_components;
using Space_Server.server;
using Space_Server.utility;

namespace Space_Server.game {
    public class Game {
        private Random _random;
        private readonly object _shopLocker = new object();
        private const int RollCost = 2;
        private const int LvlUpCost = 4;
        public ConcurrentList<NetworkClient> AllClients { get; }
        public ConcurrentList<NetworkClient> AliveClients { get; }
        public ConcurrentDictionary<NetworkClient, NetworkClient> LastOpponents { get; set; }
        public ConcurrentList<PvpFight> PvpFights { get; set; }
        public NetworkClient LastDead { get; set; }
        public List<GamePlayer> GamePlayers { get; }
        public ConcurrentList<ShipComponentType>[] Pool { get; set; }

        public Game(ConcurrentList<NetworkClient> clients) {
            AllClients = clients;
            AliveClients = new ConcurrentList<NetworkClient>();
            AliveClients.TryAddRange(clients);
            GamePlayers = AliveClients.Select(client => client.GamePlayer).ToList();
        }

        public void Start() {
            for (var i = 1; i <= 5; i++) {
                PlayRound(i);
            }
        }

        public void Generate() {
            _random = new Random();
            LastOpponents = new ConcurrentDictionary<NetworkClient, NetworkClient>();
            Pool = GeneratePool((ShipComponentType[]) Enum.GetValues(typeof(ShipComponentType)));
            foreach (var client in AliveClients) {
                client.GamePlayer.Reset();
                GamePlayers.Add(client.GamePlayer);
                RollShop(client.GamePlayer);
            }
        }

        private void PlayRound(int round) {
            PvpFights = GenerateFights(AliveClients);
            foreach (var gamePlayer in GamePlayers) {
                AddXp(gamePlayer, LvlUpCost);
                ChangeMoney(gamePlayer, 5);
                RollShop(gamePlayer);
            }
            foreach (var client in AliveClients) {
                SendRound(client, round);
                AddCommandLvlUp(client);
                AddCommandShopBuy(client);
                AddCommandShopRoll(client);
                AddCommandBuyShip(client);
                AddCommandShipReposition(client);
                AddCommandAddShipComponent(client);
                SendPhaseBuying(client);
                SendXp(client);
                if (isNewLvl(client.GamePlayer))
                    SendLvlUp(client);
                SendMoney(client);
                SendShop(client);
                SendOpponent(client);
            }

            const int buySeconds = 5;
            for (var currentSecond = 0; currentSecond < buySeconds; currentSecond++) {
                var timeLeft = buySeconds - currentSecond;
                AliveClients.ForEach(client => SendTime(client, timeLeft));
                Thread.Sleep(1000);
            }

            const int positioningSeconds = 5;
            foreach (var client in AliveClients) {
                RemoveCommandLvlUp(client);
                RemoveCommandShopBuy(client);
                RemoveCommandShopRoll(client);
                RemoveCommandBuyShip(client);
                RemoveCommandAddShipComponent(client);
                SendPhasePositioning(client);
            }
            for (var currentSecond = 0; currentSecond < positioningSeconds; currentSecond++) {
                var timeLeft = positioningSeconds - currentSecond;
                AliveClients.ForEach(client => SendTime(client, timeLeft));
                Thread.Sleep(1000);
            }

            const int fightSeconds = 5;
            foreach (var client in AliveClients) {
                RemoveCommandShipReposition(client);
                SendPhaseFighting(client);
            }
            var fightRandomSeed = _random.Next();
            var fightsRandom = new Random(fightRandomSeed);
            var fightTasks = new List<Task<FightWinner>>();
            foreach (var pvpFight in PvpFights) {
                var fightTask = new Task<FightWinner>(() => CalcWinner(pvpFight, fightSeconds, fightsRandom));
                fightTasks.Add(fightTask);
                fightTask.Start();
            }
            //todo w8 clients Loading
            foreach (var client in AliveClients) {
                SendStartFight(client, fightRandomSeed);
            }
            for (var currentSecond = 0; currentSecond < fightSeconds; currentSecond++) {
                var timeLeft = fightSeconds - currentSecond;
                Thread.Sleep(1000);
            }
            foreach (var fightTask in fightTasks) {
                fightTask.Wait();
                ChangeHp(fightTask.Result.Loser.GamePlayer, fightTask.Result.Damage);
                SendHp(fightTask.Result.Loser);
                if (fightTask.Result.Tie) {
                    ChangeHp(fightTask.Result.Winner.GamePlayer, fightTask.Result.Damage);
                    SendHp(fightTask.Result.Winner);
                }
            }
        }

        private struct FightWinner {
            public NetworkClient Winner;
            public NetworkClient Loser;
            public bool Tie;
            public int Damage;
        }

        private FightWinner CalcWinner(PvpFight pvpFight, int fightSeconds, Random random) {
            const int tickRate = 30;
            var fightShips = pvpFight.FirstPlayer.GamePlayer.SpaceShips
                .Concat(pvpFight.SecondPlayer.GamePlayer.SpaceShips)
                .Select(spaceShip => new FightShip(spaceShip))
                .ToList();
            for (var i = 0; i < fightSeconds * tickRate; i++) {
                foreach (var currentShip in fightShips) {
                    if (currentShip.BusyTicksAA == -1) {
                        if (currentShip.AfterBusySpells.Count == 0) {
                            if (currentShip.Target == null || !currentShip.Target.Alive) { //Поиск Цели
                                currentShip.BusyTicksSpell = 0;
                                currentShip.AfterBusySpells = null;
                                var minDistance = float.MaxValue;
                                FightShip minDistanceFightShip = null;
                                foreach (var opponentFightShip in fightShips.Where(ship => ship != currentShip)) {
                                    var distance = pvpFight.CalcDistance(currentShip.Ship, opponentFightShip.Ship);
                                    if (distance < minDistance) {
                                        minDistance = distance;
                                        minDistanceFightShip = opponentFightShip;
                                    }
                                }
                                currentShip.Target = minDistanceFightShip;
                                continue;
                            }
                            if (pvpFight.CalcDistance(currentShip.Ship, currentShip.Target.Ship) >
                                currentShip.AttackRange) { //Передвижение к цели
                                //лететь к цели
                                continue;
                            }
                        } else if (currentShip.BusyTicksSpell == 0) { //Использование способности
                            var spellList = currentShip.AfterBusySpells;
                            if (spellList.Count > 0) {
                                spellList[currentShip.ActiveSpellIndex].Spell(pvpFight.PvpArena, currentShip, random);
                                spellList.RemoveAt(currentShip.ActiveSpellIndex);
                                if (spellList.Count > 0) {
                                    var randomSpellIndex = random.Next(spellList.Count);
                                    var randomSpell = spellList[randomSpellIndex];
                                    currentShip.BusyTicksSpell = (int) (randomSpell.SpellCastSeconds * tickRate);
                                    currentShip.ActiveSpellIndex = randomSpellIndex;
                                }
                                continue;
                            }
                            if (currentShip.Energy >= currentShip.MaxEnergy) {
                                currentShip.Energy = 0;
                                if (currentShip.Ship.Gun != ShipComponentType.Empty) {
                                    var spell = ((Gun) ShipComponentInfo.Get(currentShip.Ship.Gun)).Spell;
                                    if (spell != null) {
                                        spellList.Add(spell);
                                    }
                                }
                                if (currentShip.Ship.Shell != ShipComponentType.Empty) {
                                    var spell = ((Shell) ShipComponentInfo.Get(currentShip.Ship.Shell)).Spell;
                                    if (spell != null) {
                                        spellList.Add(spell);
                                    }
                                }
                                if (currentShip.Ship.Reactor != ShipComponentType.Empty) {
                                    var spell = ((Reactor) ShipComponentInfo.Get(currentShip.Ship.Reactor)).Spell;
                                    if (spell != null) {
                                        spellList.Add(spell);
                                    }
                                }
                                var randomSpellIndex = random.Next(spellList.Count);
                                var randomSpell = spellList[randomSpellIndex];
                                currentShip.BusyTicksSpell = (int) (randomSpell.SpellCastSeconds * tickRate);
                                currentShip.ActiveSpellIndex = randomSpellIndex;
                                continue;
                            }
                        } else {
                            currentShip.BusyTicksSpell--;
                            continue;
                        }
                        currentShip.BusyTicksAA = (int) (currentShip.AttackSpeed * tickRate); //Начало Автоатаки
                    } else if (currentShip.BusyTicksAA == 0) { //Завершение Автоатаки
                        currentShip.Energy += currentShip.EnergyRegenPerAttack;
                        currentShip.Target.Hp -= currentShip.AttackDamage; //заменить на функцию с броней и убийством
                        currentShip.BusyTicksAA = -1;
                    } else {
                        currentShip.BusyTicksAA--;
                    }
                }
            }
            return new FightWinner {
                Winner = pvpFight.FirstPlayer,
                Loser = pvpFight.SecondPlayer,
                Tie = false,
                Damage = _random.Next(-20, -5)
            };
        }

        private void SendStartFight(NetworkClient client, int fightRandomSeed) {
            SendToClient(client, $"GAME_START_FIGHT:{fightRandomSeed}");
        }

        private void SendPhaseBuying(NetworkClient client) {
            SendToClient(client, $"GAME_PHASE_UPDATE:BUYING");
        }

        private void SendPhasePositioning(NetworkClient client) {
            SendToClient(client, $"GAME_PHASE_UPDATE:POSITIONING");
        }

        private void SendPhaseFighting(NetworkClient client) {
            SendToClient(client, $"GAME_PHASE_UPDATE:FIGHTING");
        }

        private void SendToClient(NetworkClient client, string message) {
            client.TcpSend(message);
        }

        private ConcurrentList<ShipComponentType>[] GeneratePool(ShipComponentType[] componentTypes) {
            var pool = new[] {
                new ConcurrentList<ShipComponentType>(),
                new ConcurrentList<ShipComponentType>(),
                new ConcurrentList<ShipComponentType>(),
                new ConcurrentList<ShipComponentType>(),
                new ConcurrentList<ShipComponentType>()
            };
            foreach (var componentType in componentTypes.Where(type => type != ShipComponentType.Empty)) {
                var componentInfo = ShipComponentInfo.Get(componentType);
                for (var i = 0; i < componentInfo.Tier.Count; i++) {
                    pool[componentInfo.Tier.Index].TryInsert(_random.Next(pool[componentInfo.Tier.Index].Count + 1),
                        componentType);
                }
            }
            PrintPool(pool);
            return pool;
        }

        private static void PrintPool(IReadOnlyList<ConcurrentList<ShipComponentType>> pool) {
            var mes = new[] {"1 Tier", "2 Tier", "3 Tier", "4 Tier", "5 Tier"};
            for (var i = 0; i < pool.Count; i++) {
                Log.Debug(mes[i]);
                foreach (var componentType in pool[i])
                    Log.Debug($"{componentType} {ShipComponentInfo.Get(componentType).Tier.Index + 1}");
            }
        }

        private ConcurrentList<PvpFight> GenerateFights(IReadOnlyList<NetworkClient> aliveClients) {
            var pvpFights = new ConcurrentList<PvpFight>();
            if (aliveClients.Count == 2) {
                pvpFights.Add(new PvpFight(aliveClients[0], aliveClients[1]));
            } else {
                var tempClients = new List<NetworkClient>(aliveClients);
                if (tempClients.Count % 2 != 0)
                    tempClients.Add(LastDead);
                var fighters = new List<NetworkClient>();
                for (var i = 0; i < tempClients.Count; i++) {
                    var randomIndex = _random.Next(tempClients.Count);
                    fighters.Add(tempClients[randomIndex]);
                    tempClients.RemoveAt(randomIndex);
                }
                foreach (var fighter in fighters) {
                    NetworkClient lastOpponent;
                    if (tempClients.Count == 2) {
                        LastOpponents.TryGetValue(fighters[^1], out lastOpponent);
                        var index = tempClients.IndexOf(lastOpponent);
                        if (index != -1) {
                            pvpFights.Add(CreatePvpFight(fighters[^2], tempClients[index]));
                            pvpFights.Add(CreatePvpFight(fighters[^1], tempClients[index == 0 ? 1 : 0]));
                            break;
                        }
                    }
                    LastOpponents.TryGetValue(fighter, out lastOpponent);
                    var randomIndex = _random.Next(tempClients.Count);
                    while (lastOpponent == tempClients[randomIndex]) {
                        randomIndex = _random.Next(tempClients.Count);
                    }
                    pvpFights.Add(CreatePvpFight(fighter, tempClients[randomIndex]));
                    tempClients.RemoveAt(randomIndex);
                }
            }
            pvpFights.ForEach(item => Log.Debug(item.ToString()));
            return pvpFights;
        }

        private PvpFight CreatePvpFight(NetworkClient fighter, NetworkClient opponent) {
            LastOpponents[fighter] = opponent;
            LastOpponents[opponent] = fighter;
            return new PvpFight(fighter, opponent);
        }

        private void SendOpponent(NetworkClient client) {
            var message = $"GAME_OPPONENT_UPDATE:{AliveClients.IndexOf(LastOpponents[client])}";
            client.TcpSend(message);
        }

        private void RollShop(GamePlayer player) {
            lock (_shopLocker) {
                var tierList = GetTiers(player);
                var oldItems = new List<ShipComponentType>();
                var newItems = new ShipComponentType[player.Shop.Length];
                for (var i = 0; i < player.Shop.Length; i++) {
                    if (player.Shop[i] != ShipComponentType.Empty)
                        oldItems.Add(player.Shop[i]);
                    if (Pool[tierList[i]].TryPop(out var item))
                        newItems[i] = item;
                    else
                        Log.Print("SHOP BUG: CAN'T ROLL");
                }
                oldItems.ForEach(ReturnToPool);
                player.Shop = newItems;
            }
        }

        private int[] GetTiers(GamePlayer player) {
            var shopLength = player.Shop.Length;
            var resultChances = new int[shopLength];
            var tierChances = Tier.ChancesByLvl[player.Level];
            for (var i = 0; i < shopLength; i++) {
                var randomNum = _random.Next(1, 101);
                var temp = 0;
                for (var j = 0; j < tierChances.Length; j++) {
                    var tierChance = tierChances[j];
                    if (randomNum <= tierChance + temp) {
                        while (Pool[j].Count - resultChances.Count(item => item == j) == 0)
                            j--;
                        resultChances[i] = j;
                        break;
                    }
                    temp += tierChance;
                }
            }
            Log.Debug($"[{string.Join(" ", resultChances)}]");
            return resultChances;
        }

        private void ReturnToPool(ShipComponentType componentType) {
            var componentInfo = ShipComponentInfo.Get(componentType);
            Pool[componentInfo.Tier.Index].TryInsert(
                _random.Next(Pool[componentInfo.Tier.Index].Count + 1), componentType
            );
        }

        private void SendShop(NetworkClient client) {
            var message = "GAME_SHOP_UPDATE:";
            foreach (var componentType in client.GamePlayer.Shop)
                message = message + $"{componentType} ";
            message = message.Remove(message.Length - 1);
            client.TcpSend(message);
        }

        private IntVector2 AddShipByType(GamePlayer player, HullType hullType) {
            var newShip = new SpaceShip(hullType);
            player.SpaceShips.Add(newShip);
            var coordinates = player.PersonArena.Arena.CoordinatesOf(null);
            player.PersonArena.Arena[coordinates.X, coordinates.Y] = newShip;
            return coordinates;
        }

        private void SendAddShip(NetworkClient client, HullType hullType, int newX, int newY) {
            var message = $"GAME_ADD_SHIP:{hullType} {newX} {newY}";
            client.TcpSend(message);
        }

        private void SendTime(NetworkClient client, int time) {
            var message = $"GAME_TIME_UPDATE:{time}";
            client.TcpSend(message);
        }

        private void SendRound(NetworkClient client, int round) {
            var message = $"GAME_ROUND_UPDATE:{round}";
            client.TcpSend(message);
        }

        private void ChangeMoney(GamePlayer player, int money) {
            player.Money += money;
        }

        private void SendMoney(NetworkClient client) {
            var message = $"GAME_MONEY_UPDATE:{client.GamePlayer.Money}";
            client.TcpSend(message);
        }

        private void ChangeHp(GamePlayer player, int hp) {
            player.Hp += hp;
        }

        private void SendHp(NetworkClient client) {
            var message = $"GAME_HP_UPDATE:{client.GamePlayer.Hp}";
            client.TcpSend(message);
        }

        private void AddXp(GamePlayer player, int xp) {
            if (player.Level < 5) {
                player.Xp += xp;
                if (player.Xp >= 12) {
                    player.Xp = 0;
                    player.Level += 1;
                }
            }
        }

        private void SendXp(NetworkClient client) {
            var message = $"GAME_XP_UPDATE:{client.GamePlayer.Xp}";
            client.TcpSend(message);
        }

        private bool isNewLvl(GamePlayer player) => player.Xp == 0;

        private void SendLvlUp(NetworkClient client) {
            var message = $"GAME_LVL_UP:{client.GamePlayer.Level}";
            client.TcpSend(message);
        }

        private void ShipReposition(GamePlayer player, int shipIndex, int newX, int newY) {
            var oldCoordinates = player.PersonArena.Arena.CoordinatesOf(player.SpaceShips[shipIndex]);
            player.PersonArena.Arena[newX, newY] = player.PersonArena.Arena[oldCoordinates.X, oldCoordinates.Y];
            player.PersonArena.Arena[oldCoordinates.X, oldCoordinates.Y] = null;
        }

        private void SendShipReposition(NetworkClient client, int shipIndex, int newX, int newY) {
            var message = $"GAME_SHIP_REPOSITION:{shipIndex} {newX} {newY}";
            client.TcpSend(message);
        }

        private bool CanBuyComponent(GamePlayer player, int shopIndex) {
            //todo T2 GUNS checking
            var hasItem = player.Shop[shopIndex] != ShipComponentType.Empty;
            if (hasItem) {
                var enoughMoney = player.Money >= ShipComponentInfo.Get(player.Shop[shopIndex]).Tier.Cost;
                var hasPlace = player.BoughtComponents.Count(
                    component => component == ShipComponentType.Empty
                ) > 0;
                return enoughMoney && hasPlace;
            }
            return false;
        }

        private ShipComponentType BuyComponent(GamePlayer player, int shopIndex) {
            var componentType = player.Shop[shopIndex];
            ChangeMoney(player, ShipComponentInfo.Get(componentType).Tier.Cost);
            player.Shop[shopIndex] = ShipComponentType.Empty;
            return componentType;
        }

        private void SendBuyComponent(NetworkClient client, int shopIndex) {
            var message = $"GAME_BUY:{shopIndex}";
            client.TcpSend(message);
        }

        private void AddBoughtComponent(GamePlayer player, ShipComponentType componentType) {
            //todo MAKE T2 Guns
            var index = Array.IndexOf(player.BoughtComponents, ShipComponentType.Empty);
            player.BoughtComponents[index] = componentType;
        }

        private void SendAddBoughtComponent(NetworkClient client, ShipComponentType componentType) {
            var message = $"GAME_ADD_COMPONENT:{componentType}";
            client.TcpSend(message);
        }

        private void AddShipComponent(SpaceShip ship, ShipComponentType componentType) {
            var componentInfo = ShipComponentInfo.Get(componentType);
            if (componentInfo is Gun) {
                if (ship.Gun != ShipComponentType.Empty) {
                    ReturnToPool(ship.Gun);
                }
                ship.Gun = componentType;
            } else if (componentInfo is Shell) {
                if (ship.Shell != ShipComponentType.Empty) {
                    ReturnToPool(ship.Shell);
                }
                ship.Shell = componentType;
            } else if (componentInfo is Reactor) {
                if (ship.Reactor != ShipComponentType.Empty) {
                    ReturnToPool(ship.Reactor);
                }
                ship.Reactor = componentType;
            }
        }

        private void SendAddShipComponent(NetworkClient client, int shipIndex, ShipComponentType componentType) {
            var message = $"GAME_ADD_SHIP_COMPONENT:{shipIndex} {componentType}";
            client.TcpSend(message);
        }

        private void AddCommandShopRoll(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_SHOP_ROLL", args => {
                if (client.GamePlayer.Money >= RollCost) {
                    ChangeMoney(client.GamePlayer, -2);
                    RollShop(client.GamePlayer);
                    SendShop(client);
                }
            });
        }

        private void AddCommandShopBuy(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_SHOP_BUY", args => {
                var index = int.Parse(args[0]);
                if (CanBuyComponent(client.GamePlayer, index)) {
                    var componentType = BuyComponent(client.GamePlayer, index);
                    SendBuyComponent(client, index);
                    SendMoney(client);
                    AddBoughtComponent(client.GamePlayer, componentType);
                    SendAddBoughtComponent(client, componentType);
                }
            });
        }

        private void RemoveCommandShopBuy(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_SHOP_BUY");
        }

        private void RemoveCommandShopRoll(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_SHOP_ROLL");
        }

        private void AddCommandLvlUp(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_LVL_UP", args => {
                var player = client.GamePlayer;
                if (player.Money >= LvlUpCost) {
                    AddXp(player, LvlUpCost);
                    if (isNewLvl(player))
                        SendLvlUp(client);
                    else
                        SendXp(client);
                }
            });
        }

        private void RemoveCommandLvlUp(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_LVL_UP");
        }

        private void AddCommandBuyShip(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_BUY_SHIP", args => {
                var player = client.GamePlayer;
                if (player.SpaceShips.Count < player.Level) {
                    var hullType = (HullType) int.Parse(args[0]);
                    var coordinates = AddShipByType(player, hullType);
                    SendAddShip(client, hullType, coordinates.X, coordinates.Y);
                }
            });
        }

        private void RemoveCommandBuyShip(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_BUY_SHIP");
        }

        private void AddCommandShipReposition(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_CHANGE_SHIP_POS", args => {
                var shipIndex = int.Parse(args[0]);
                var newX = int.Parse(args[1]);
                var newY = int.Parse(args[2]);
                var player = client.GamePlayer;
                if (player.PersonArena.Arena[newX, newY] == null) {
                    ShipReposition(player, shipIndex, newX, newY);
                    SendShipReposition(client, shipIndex, newX, newY);
                }
            });
        }

        private void RemoveCommandShipReposition(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_CHANGE_SHIP_POS");
        }

        private void AddCommandAddShipComponent(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_ADD_SHIP_COMPONENT", args => { });
        }

        private void RemoveCommandAddShipComponent(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_ADD_SHIP_COMPONENT");
        }
    }
}