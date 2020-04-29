using System;
using System.Collections.Generic;
using System.Linq;
using Space_Server.model;
using Space_Server.server;
using Space_Server.utility;

namespace Space_Server.game {
    public class Game {
        private readonly object _shopLocker = new object();
        private Random _random;

        public Game(ConcurrentList<NetworkClient> clients) {
            Clients = clients;
        }

        public ConcurrentList<NetworkClient> Clients { get; }
        public ConcurrentList<GamePlayer> GamePlayers { get; set; }
        public ConcurrentList<ShipComponent>[] Pool { get; set; }

        public void Start() {
            Clients.ForEach(SendShop);
        }

        public void Generate() {
            _random = new Random();
            GamePlayers = new ConcurrentList<GamePlayer>();
            Pool = GeneratePool(TierInfo.TierList);
            foreach (var client in Clients) {
                client.GamePlayer.Reset();
                GamePlayers.Add(client.GamePlayer);
                RollShop(client.GamePlayer);
            }
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
                Console.WriteLine(mes[i]);
                var tier = pool[i];
                foreach (var component in tier)
                    Console.WriteLine($"{component} {component.Tier}");
            }
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
                var randomNum = _random.Next(1, 100);
                var temp = 0;
                for (var j = 0; j < tierChances.Length; j++) {
                    var tierChance = tierChances[j];
                    if (randomNum <= tierChance + temp) {
                        while (Pool[j].Count == 0)
                            j--;
                        resultChances[i] = j;
                        break;
                    }
                    temp += tierChance;
                }
            }
            Console.WriteLine(string.Join(" ", resultChances));
            return resultChances;
        }

        private void ReturnToPool(ShipComponent component) {
            Pool[component.Tier].TryInsert(_random.Next(Pool[component.Tier].Count), component);
        }

        private void SendShop(NetworkClient client) {
            var message = client.GamePlayer.Shop.Aggregate("GAME_SHOP_UPDATE:",
                (current, item) => current + $"{item.GetType().Name} ");
            message = message.Remove(message.Length - 1);
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
                player.Level = 0;
                return true;
            }
            return false;
        }

        private void SendLvlUp(NetworkClient client) {
            var message = $"GAME_LVL_UP:{client.GamePlayer.Hp}";
            client.TcpSend(message);
        }

        private void AddXp(GamePlayer player) {
            player.Xp += 4;
        }

        private void SendXp(NetworkClient client) {
            var message = $"GAME_XP_UPDATE:{client.GamePlayer.Xp}";
            client.TcpSend(message);
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
            player.BoughtComponents.Add(component);
        }

        private void SendAddBoughtComponent(NetworkClient client, ShipComponent component) {
            var message = $"GAME_ADD_COMPONENT:{component}";
            client.TcpSend(message);
        }

        private void AddShopCommands(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_SHOP_ROLL", args => {
                RollShop(client.GamePlayer);
                SendShop(client);
            });
            client.AddCommand(CommandType.GAME, "GAME_SHOP_BUY", args => {
                var index = int.Parse(args[0]);
                var component = BuyComponent(client.GamePlayer, index);
                SendBuyComponent(client, index);
                SendMoney(client);
                AddBoughtComponent(client.GamePlayer, component);
                SendAddBoughtComponent(client, component);
            });
        }

        private void RemoveShopCommand(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_SHOP_ROLL");
            client.RemoveCommand(CommandType.GAME, "GAME_SHOP_BUY");
        }

        private void AddLvlUpCommand(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_LVL_UP", args => {
                AddXp(client.GamePlayer);
                if (LvlUp(client.GamePlayer))
                    SendLvlUp(client);
                else
                    SendXp(client);
            });
        }

        private void RemoveLvlUpCommand(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_LVL_UP");
        }
    }
}