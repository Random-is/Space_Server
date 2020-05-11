using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using Space_Server.model;
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
        public NetworkClient LastDead { get; set; }
        public List<GamePlayer> GamePlayers => AliveClients.Select(client => client.GamePlayer).ToList();
        public ConcurrentList<ShipComponent>[] Pool { get; set; }
        public ConcurrentList<PvpFight> PvpFights { get; set; }

        public Game(ConcurrentList<NetworkClient> clients) {
            AllClients = clients;
            AliveClients = new ConcurrentList<NetworkClient>();
            AliveClients.TryAddRange(clients);
        }

        public void Start() {
            for (var i = 1; i <= 10; i++) {
                PlayRound(i);
            }
        }

        public void Generate() {
            _random = new Random();
            LastOpponents = new ConcurrentDictionary<NetworkClient, NetworkClient>();
            Pool = GeneratePool(TierInfo.TierList);
            foreach (var client in AliveClients) {
                client.GamePlayer.Reset();
                GamePlayers.Add(client.GamePlayer);
                RollShop(client.GamePlayer);
            }
        }

        private void PlayRound(int round) {
            PvpFights = GenerateFights(AliveClients);
            foreach (var gamePlayer in GamePlayers) {
                AddXp(gamePlayer);
                LvlUp(gamePlayer);
                ChangeMoney(gamePlayer, 5);
                RollShop(gamePlayer);
            }
            foreach (var client in AliveClients) {
                SendRound(client, ConvertRound(round));
                AddCommandLvlUp(client);
                AddCommandsShop(client);
                SendPhaseBuying(client);
                SendXp(client);
                if (client.GamePlayer.Xp == 0) 
                    SendLvlUp(client);
                SendMoney(client);
                SendShop(client);
                SendOpponent(client);
                
            }
            const int buyTime = 20;
            for (var currentSecond = 0; currentSecond < buyTime; currentSecond++) {
                var timeLeft = buyTime - currentSecond;
                AliveClients.ForEach(client => SendTime(client, timeLeft));
                Thread.Sleep(1000);
            }
            const int positioningTime = 15;
            foreach (var client in AliveClients) {
                RemoveCommandsShop(client);
                RemoveCommandLvlUp(client);
                AddCommandShipPosition(client);
                SendPhasePositioning(client);
            }
            for (var currentSecond = 0; currentSecond < positioningTime; currentSecond++) {
                var timeLeft = positioningTime - currentSecond;
                AliveClients.ForEach(client => SendTime(client, timeLeft));
                Thread.Sleep(1000);
            }
            foreach (var client in AliveClients) {
                RemoveCommandShipPosition(client);
                SendPhaseFighting(client);
            }
            for (var currentSecond = 0; currentSecond < positioningTime; currentSecond++) {
                var timeLeft = positioningTime - currentSecond;
                AliveClients.ForEach(client => SendTime(client, timeLeft));
                Thread.Sleep(1000);
            }
            foreach (var client in AliveClients) {
                ChangeHp(client.GamePlayer, _random.Next(-20, -5));
                SendHp(client);
            }
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

        private ConcurrentList<ShipComponent>[] GeneratePool(IReadOnlyList<TierInfo> tierList) {
            var pool = new ConcurrentList<ShipComponent>[5];
            for (var i = 0; i < pool.Length; i++) {
                var currentTierList = new List<ShipComponent>();
                var currentTierInfo = tierList[i];
                foreach (var type in currentTierInfo.Types)
                    for (var k = 0; k < currentTierInfo.InstanceCount; k++)
                        currentTierList.Add((ShipComponent) Activator.CreateInstance(type));
                var tempConcurrentList = new ConcurrentList<ShipComponent>();
                foreach (var item in currentTierList.OrderBy(a => Guid.NewGuid())) tempConcurrentList.TryAdd(item);
                pool[i] = tempConcurrentList;
            }
            PrintPool(pool);
            return pool;
        }

        private static void PrintPool(IReadOnlyList<ConcurrentList<ShipComponent>> pool) {
            var mes = new[] {"1 Tier", "2 Tier", "3 Tier", "4 Tier", "5 Tier"};
            for (var i = 0; i < pool.Count; i++) {
                Log.Debug(mes[i]);
                var tier = pool[i];
                foreach (var component in tier)
                    Log.Debug($"{component} {component.Tier}");
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
                var oldItems = new List<ShipComponent>();
                var newItems = new ShipComponent[player.Shop.Length];
                for (var i = 0; i < player.Shop.Length; i++) {
                    if (player.Shop[i] != null)
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
            var tierChances = TierInfo.TierChancesByLvl[player.Level];
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

        private void ReturnToPool(ShipComponent component) {
            Pool[component.Tier].TryInsert(_random.Next(Pool[component.Tier].Count + 1), component);
        }

        private void SendShop(NetworkClient client) {
            Log.Debug(string.Join(" ", client.GamePlayer.Shop.ToList()));
            var message = "GAME_SHOP_UPDATE:";
            foreach (var component in client.GamePlayer.Shop) 
                message = message + $"{component.GetType().Name} ";
            message = message.Remove(message.Length - 1);
            client.TcpSend(message);
        }

        private string ConvertTime(int time) {
            return time.ToString();
        }

        private void SendTime(NetworkClient client, int time) {
            var message = $"GAME_TIME_UPDATE:{time}";
            client.TcpSend(message);
        }

        private string ConvertRound(int round) {
            return round.ToString();
        }

        private void SendRound(NetworkClient client, string round) {
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

        private bool LvlUp(GamePlayer player) {
            if (player.Xp >= 12) {
                player.Xp = 0;
                player.Level += 1;
                return true;
            }
            return false;
        }

        private void SendLvlUp(NetworkClient client) {
            var message = $"GAME_LVL_UP:{client.GamePlayer.Level}";
            client.TcpSend(message);
        }

        private void AddXp(GamePlayer player) {
            player.Xp += LvlUpCost;
        }

        private void SendXp(NetworkClient client) {
            var message = $"GAME_XP_UPDATE:{client.GamePlayer.Xp}";
            client.TcpSend(message);
        }

        private bool CanBuyComponent(GamePlayer player, int index) {
            //todo T2 GUNS checking
            var hasItem = player.Shop[index] != null;
            if (hasItem) {
                var enoughMoney = player.Money >= player.Shop[index].Tier + 1;
                var hasPlace = player.BoughtComponents.Count < 8;
                return enoughMoney && hasPlace;
            }
            return false;
        }

        private ShipComponent BuyComponent(GamePlayer player, int index) {
            var component = player.Shop[index];
            ChangeMoney(player, component.Tier + 1);
            player.Shop[index] = null;
            return component;
        }

        private void SendBuyComponent(NetworkClient client, int index) {
            var message = $"GAME_BUY:{index}";
            client.TcpSend(message);
        }

        private void AddBoughtComponent(GamePlayer player, ShipComponent component) {
            //todo MAKE T2 Guns
            player.BoughtComponents.Add(component);
        }

        private void SendAddBoughtComponent(NetworkClient client, ShipComponent component) {
            var message = $"GAME_ADD_COMPONENT:{component}";
            client.TcpSend(message);
        }

        private void AddCommandsShop(NetworkClient client) {
            AddCommandShopRoll(client);
            AddCommandShopBuy(client);
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
                    var component = BuyComponent(client.GamePlayer, index);
                    SendBuyComponent(client, index);
                    SendMoney(client);
                    AddBoughtComponent(client.GamePlayer, component);
                    SendAddBoughtComponent(client, component);
                }
            });
        }
        
        private void RemoveCommandsShop(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_SHOP_ROLL");
            client.RemoveCommand(CommandType.GAME, "GAME_SHOP_BUY");
        }

        private void AddCommandLvlUp(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_LVL_UP", args => {
                if (client.GamePlayer.Money >= LvlUpCost) {
                    AddXp(client.GamePlayer);
                    if (LvlUp(client.GamePlayer))
                        SendLvlUp(client);
                    else
                        SendXp(client);
                }
            });
        }

        private void RemoveCommandLvlUp(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_LVL_UP");
        }
        
        private void AddCommandShipPosition(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_CHANGE_SHIP_POS", args => {
                
            });
        }
        
        private void RemoveCommandShipPosition(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_CHANGE_SHIP_POS");
        }
    }
}