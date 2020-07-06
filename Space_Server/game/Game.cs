using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Game_Elements;
using Game_Elements.arena;
using Game_Elements.fight;
using Game_Elements.ship;
using Game_Elements.ship.ship_part;
using Game_Elements.utility;
using Space_Server.server;
using Space_Server.utility;

namespace Space_Server.game {
    public class Game {
        private Random _random;
        private readonly object _shopLocker = new object();
        private const int RollCost = 2;
        private const int LvlUpCost = 5;
        public ConcurrentList<NetworkClient> AllClients { get; }
        public ConcurrentList<NetworkClient> AliveClients { get; }
        public ConcurrentDictionary<NetworkClient, NetworkClient> LastOpponents { get; set; }
        public ConcurrentList<PvpFight> PvpFights { get; set; }
        public NetworkClient LastDead { get; set; }
        public ConcurrentList<GamePlayer> GamePlayers { get; set; }
        public PartsPool<ConcurrentList<ShipPart>> Pool { get; set; }

        public Game(ConcurrentList<NetworkClient> clients) {
            AllClients = clients;
            AliveClients = new ConcurrentList<NetworkClient>();
            AliveClients.TryAddRange(clients);
        }

        public void Start() {
            for (var i = 1; i <= 5; i++) {
                PlayRound(i);
            }
        }

        public void Generate() {
            _random = new Random();
            LastOpponents = new ConcurrentDictionary<NetworkClient, NetworkClient>();
            GamePlayers = new ConcurrentList<GamePlayer>();
            Pool = new PartsPool<ConcurrentList<ShipPart>>();
            Pool.Generate(ShipPartInfo.All.Values, _random);
            foreach (var client in AliveClients) {
                client.GamePlayer.Reset();
                GamePlayers.Add(client.GamePlayer);
            }
        }

        private void PlayRound(int round) {
            //TODO Баг с тем что оне удаляется GamePlayer при смерти
            PvpFights = GenerateFights(AliveClients);
            foreach (var gamePlayer in GamePlayers) {
                gamePlayer.AddXp(1);
                gamePlayer.ChangeMoney(5);
                if (!gamePlayer.ShopLock)
                    RollShop(gamePlayer);
            }
            foreach (var client in AliveClients) {
                SendRound(client, round);
                SendGamePlayersHp(client);
                SendXp(client);
                if (client.GamePlayer.IsNewLvl())
                    SendLvlUp(client);
                SendMoney(client);
                SendShop(client);
                AddCommandLvlUp(client);
                AddCommandShopBuy(client);
                AddCommandShopRoll(client);
                AddCommandBuyShip(client);
                AddCommandShipReposition(client);
                AddCommandAddShipPart(client);
                AddCommandShopLock(client);
                AddCommandBagItemReposition(client);
                AddCommandSellBagItem(client);
                SendPhaseBuying(client);
                SendOpponent(client);
            }

            const int buySeconds = 40;
            for (var currentSecond = 0; currentSecond < buySeconds; currentSecond++) {
                var timeLeft = buySeconds - currentSecond;
                if (currentSecond % (buySeconds / 3) == 0) {
                    AliveClients.ForEach(client => SendTime(client, timeLeft));
                }
                Thread.Sleep(1000);
            }

            const int positioningSeconds = 10;
            foreach (var client in AliveClients) {
                RemoveCommandLvlUp(client);
                RemoveCommandShopBuy(client);
                RemoveCommandShopRoll(client);
                RemoveCommandBuyShip(client);
                RemoveCommandAddShipPart(client);
                RemoveCommandShopLock(client);
                RemoveCommandBagItemReposition(client);
                RemoveCommandSellBagItem(client);
                SendPhasePositioning(client);
            }
            for (var currentSecond = 0; currentSecond < positioningSeconds; currentSecond++) {
                var timeLeft = positioningSeconds - currentSecond;
                if (currentSecond % (positioningSeconds / 3) == 0) {
                    AliveClients.ForEach(client => SendTime(client, timeLeft));
                }
                Thread.Sleep(1000);
            }

            const int fightDurationSeconds = 20;
            foreach (var client in AliveClients) {
                RemoveCommandShipReposition(client);
                SendPhaseFighting(client);
            }
            var fightRandomSeed = _random.Next();
            var fightTasks = new List<Task<FightResult>>();
            foreach (var pvpFight in PvpFights) {
                var fightTask = new Task<FightResult>(() =>
                    Fight.CalcWinner(pvpFight.PvpArena, fightDurationSeconds, new Random(fightRandomSeed)));
                fightTasks.Add(fightTask);
                fightTask.Start();
            }
            //todo w8 clients Loading
            foreach (var client in AliveClients) {
                SendStartFight(client, fightRandomSeed);
            }
            for (var currentSecond = 0; currentSecond < fightDurationSeconds; currentSecond++) {
                var timeLeft = fightDurationSeconds - currentSecond;
                Thread.Sleep(1000);
            }
            foreach (var fightTask in fightTasks) {
                fightTask.Wait();
                var loserClient = AliveClients.Find(client => client.GamePlayer == fightTask.Result.Loser);
                loserClient?.GamePlayer.ChangeHp(-fightTask.Result.Damage);
                // SendHp(loserClient);
                if (fightTask.Result.Tie) {
                    var winnerClient = AliveClients.Find(client => client.GamePlayer == fightTask.Result.Winner);
                    winnerClient?.GamePlayer.ChangeHp(-fightTask.Result.Damage);
                    // SendHp(winnerClient);
                }
            }
        }

        private void SendGamePlayersHp(NetworkClient client) {
            SendToClient(
                client,
                $"GAME_PLAYERS_HP_UPDATE:{string.Join(' ', GamePlayers.Select(player => player.Hp))}"
            );
        }

        private void SendStartFight(NetworkClient client, int fightRandomSeed) {
            var pvpFight = PvpFights.Find(fight => fight.MainPlayer == client || fight.OpponentPlayer == client);
            var playerIndex = -1;
            GamePlayer opponent;
            if (pvpFight.MainPlayer == client) {
                opponent = pvpFight.OpponentPlayer.GamePlayer;
                playerIndex = 0;
            } else {
                opponent = pvpFight.MainPlayer.GamePlayer;
                playerIndex = 1;
            }
            var opponentShipsInfo = "";
            foreach (var opponentShip in opponent.Ships) {
                opponentShipsInfo +=
                    $"{(int) opponentShip.Hull.Name} " +
                    $"{(opponentShip.Parts[ShipPartType.Gun] != null ? (int) opponentShip.Parts[ShipPartType.Gun].Name : -1)} " +
                    $"{(opponentShip.Parts[ShipPartType.Reactor] != null ? (int) opponentShip.Parts[ShipPartType.Reactor].Name : -1)} " +
                    $"{(opponentShip.Parts[ShipPartType.Facing] != null ? (int) opponentShip.Parts[ShipPartType.Facing].Name : -1)} ";
                var position = opponent.PlayerArena.Arena.CoordinatesOf(opponentShip);
                opponentShipsInfo += $"{position.Y} {position.X} ";
            }
            SendToClient(
                client,
                $"GAME_START_FIGHT:{fightRandomSeed} {playerIndex} {opponent.Ships.Count} {opponentShipsInfo}"
            );
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

        private ConcurrentList<PvpFight> GenerateFights(IReadOnlyList<NetworkClient> aliveClients) {
            var pvpFights = new ConcurrentList<PvpFight>();
            if (aliveClients.Count == 2) {
                pvpFights.Add(CreatePvpFight(aliveClients[0], aliveClients[1]));
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
                var rollTiers = Pool.GetRollTiers(player, _random);
                Log.Debug($"[{string.Join(" ", rollTiers)}]");
                var oldItems = new List<ShipPart>();
                var newItems = new ShipPart[player.Shop.Length];
                for (var i = 0; i < player.Shop.Length; i++) {
                    if (player.Shop[i] != null)
                        oldItems.Add(player.Shop[i]);
                    if (Pool[rollTiers[i]].TryPop(out var item))
                        newItems[i] = item;
                    else
                        Log.Print("SHOP BUG: CAN'T ROLL");
                }
                oldItems.ForEach(ReturnToPool);
                player.Shop = newItems;
            }
        }

        private void ReturnToPool(ShipPart shipPart) {
            Pool[shipPart.TierName].TryInsert(
                _random.Next(Pool[shipPart.TierName].Count + 1),
                shipPart
            );
        }

        private void SendShop(NetworkClient client) {
            var message = "GAME_SHOP_UPDATE:";
            foreach (var componentType in client.GamePlayer.Shop) {
                message = message + $"{(componentType != null ? (int) componentType.Name : -1)} ";
            }
            message = message.Remove(message.Length - 1);
            client.TcpSend(message);
        }

        private void SendAddShip(NetworkClient client, ShipHullName shipHullName, int newY, int newX) {
            var message = $"GAME_ADD_SHIP:{(int) shipHullName} {newY} {newX}";
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

        private void SendMoney(NetworkClient client) {
            var message = $"GAME_MONEY_UPDATE:{client.GamePlayer.Money}";
            client.TcpSend(message);
        }

        private void SendHp(NetworkClient client) {
            var message = $"GAME_HP_UPDATE:{client.GamePlayer.Hp}";
            client.TcpSend(message);
        }

        private void SendXp(NetworkClient client) {
            var message = $"GAME_XP_UPDATE:{client.GamePlayer.Xp}";
            client.TcpSend(message);
        }

        private void SendLvlUp(NetworkClient client) {
            var message = $"GAME_LVL_UP:{client.GamePlayer.Lvl}";
            client.TcpSend(message);
        }

        private void SendShipReposition(NetworkClient client, int shipIndex, int newY, int newX) {
            var message = $"GAME_SHIP_REPOSITION:{shipIndex} {newY} {newX}";
            client.TcpSend(message);
        }

        private void SendBuyComponent(NetworkClient client, int shopIndex) {
            var message = $"GAME_SHOP_BUY:{shopIndex}";
            client.TcpSend(message);
        }

        private void SendAddBagComponent(NetworkClient client, ShipPart shipPart) {
            var message = $"GAME_ADD_BAG_ITEM:{(int) shipPart.Name}";
            client.TcpSend(message);
        }

        private void AddShipComponent(Ship ship, ShipPart newShipPart) {
            ReturnToPool(ship.Parts[newShipPart.Type]);
            ship.Parts[newShipPart.Type] = newShipPart;
        }

        private void SendAddPartToShip(NetworkClient client, int shipIndex, int bagIndex) {
            var message = $"GAME_ADD_PART_TO_SHIP:{shipIndex} {bagIndex}";
            client.TcpSend(message);
        }

        private void AddCommandShopRoll(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_SHOP_ROLL", args => {
                if (client.GamePlayer.Money >= RollCost) {
                    client.GamePlayer.ChangeMoney(-2);
                    SendMoney(client);
                    RollShop(client.GamePlayer);
                    SendShop(client);
                }
            });
        }
        
        private void AddCommandSellBagItem(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_SELL_BAG_ITEM", args => {
                var bagIndex = int.Parse(args[0]);
                var player = client.GamePlayer;
                if (player.Bag[bagIndex] != null) {
                    client.GamePlayer.SellBagItem(bagIndex);
                    SendSellBagItem(client, bagIndex);
                    SendMoney(client);
                }
            });
        }

        private void SendSellBagItem(NetworkClient client, int bagIndex) {
            SendToClient(client, $"GAME_SELL_BAG_ITEM:{bagIndex}");
        }

        private void RemoveCommandSellBagItem(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_SELL_BAG_ITEM");
        }

        private void AddCommandShopBuy(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_SHOP_BUY", args => {
                var index = int.Parse(args[0]);
                if (client.GamePlayer.CanBuyShipPart(index)) {
                    var component = client.GamePlayer.BuyShipPart(index);
                    SendBuyComponent(client, index);
                    SendMoney(client);
                    client.GamePlayer.AddBagItem(component);
                    SendAddBagComponent(client, component);
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
                    player.AddXp(1);
                    player.ChangeMoney(-5);
                    if (player.IsNewLvl())
                        SendLvlUp(client);
                    else
                        SendXp(client);
                    SendMoney(client);
                }
            });
        }

        private void RemoveCommandLvlUp(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_LVL_UP");
        }

        private void AddCommandBuyShip(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_BUY_SHIP", args => {
                var player = client.GamePlayer;
                if (player.Ships.Count < player.Lvl + 1) {
                    var hullType = (ShipHullName) int.Parse(args[0]);
                    var newShip = new Ship(ShipHullInfo.All[hullType]);
                    var coordinates = player.AddShipToFreePosition(newShip);
                    SendAddShip(client, hullType, coordinates.Y, coordinates.X);
                }
            });
        }

        private void RemoveCommandBuyShip(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_BUY_SHIP");
        }

        private void AddCommandShipReposition(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_CHANGE_SHIP_POS", args => {
                var shipIndex = int.Parse(args[0]);
                var newY = int.Parse(args[1]);
                var newX = int.Parse(args[2]);
                var player = client.GamePlayer;
                var targetShip = player.ShipReposition(player.Ships[shipIndex], newY, newX);
                SendShipReposition(client, shipIndex, newY, newX);
                // if (targetShip != null) {
                //     SendShipReposition(client, player.Ships.IndexOf(targetShip), newY, newX);
                // }
            });
        }

        private void RemoveCommandShipReposition(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_CHANGE_SHIP_POS");
        }

        private void AddCommandBagItemReposition(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_CHANGE_BAG_ITEM_POS", args => {
                var oldX = int.Parse(args[0]);
                var newX = int.Parse(args[1]);
                var player = client.GamePlayer;
                player.BagItemReposition(oldX, newX);
                SendBagItemReposition(client, oldX, newX);
            });
        }

        private void SendBagItemReposition(NetworkClient client, in int oldX, in int newX) {
            SendToClient(client, $"GAME_CHANGE_BAG_ITEM_POS:{oldX} {newX}");
        }

        private void RemoveCommandBagItemReposition(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_CHANGE_BAG_ITEM_POS");
        }

        private void AddCommandShopLock(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_SHOP_LOCK", args => {
                var player = client.GamePlayer;
                player.ShopLock = !player.ShopLock;
                if (player.ShopLock) {
                    SendShopLock(client);
                } else {
                    SendShopUnlock(client);
                }
            });
        }

        private void RemoveCommandShopLock(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_SHOP_LOCK");
        }

        private void SendShopLock(NetworkClient client) {
            SendToClient(client, $"GAME_SHOP_LOCK:");
        }

        private void SendShopUnlock(NetworkClient client) {
            SendToClient(client, $"GAME_SHOP_UNLOCK:");
        }


        private void AddCommandAddShipPart(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_ADD_SHIP_PART", args => {
                var shipIndex = int.Parse(args[0]);
                var bagIndex = int.Parse(args[1]);
                var player = client.GamePlayer;
                if (player.Bag[bagIndex] != null && shipIndex <= player.Ships.Count) {
                    player.AddShipPartToShipAndSell(player.Ships[shipIndex], bagIndex);
                    SendAddPartToShip(client, shipIndex, bagIndex);
                    SendMoney(client);
                }
            });
        }

        private void RemoveCommandAddShipPart(NetworkClient client) {
            client.RemoveCommand(CommandType.GAME, "GAME_ADD_SHIP_COMPONENT");
        }
    }
}