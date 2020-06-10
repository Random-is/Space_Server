using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Game_Elements;
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
        private const int LvlUpCost = 4;
        public ConcurrentList<NetworkClient> AllClients { get; }
        public ConcurrentList<NetworkClient> AliveClients { get; }
        public ConcurrentDictionary<NetworkClient, NetworkClient> LastOpponents { get; set; }
        public ConcurrentList<PvpFight> PvpFights { get; set; }
        public NetworkClient LastDead { get; set; }
        public List<GamePlayer> GamePlayers { get; }
        public PartsPool<ConcurrentList<ShipPart>> Pool { get; set; }

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
            Pool = new PartsPool<ConcurrentList<ShipPart>>();
            Pool.Generate(ShipPartInfo.All.Values, _random);
            foreach (var client in AliveClients) {
                client.GamePlayer.Reset();
                GamePlayers.Add(client.GamePlayer);
                RollShop(client.GamePlayer);
            }
        }

        private void PlayRound(int round) {
            PvpFights = GenerateFights(AliveClients);
            foreach (var gamePlayer in GamePlayers) {
                gamePlayer.AddXp(LvlUpCost);
                gamePlayer.ChangeMoney(5);
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
                if (client.GamePlayer.IsNewLvl())
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

            const int fightDurationSeconds = 5;
            foreach (var client in AliveClients) {
                RemoveCommandShipReposition(client);
                SendPhaseFighting(client);
            }
            var fightRandomSeed = _random.Next();
            var fightTasks = new List<Task<FightResult>>();
            foreach (var pvpFight in PvpFights) {
                var fightTask = new Task<FightResult>(() => Fight.CalcWinner(pvpFight.FirstPlayer.GamePlayer, pvpFight.SecondPlayer.GamePlayer, pvpFight.PvpArena, fightDurationSeconds, new Random(fightRandomSeed)));
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
                loserClient.GamePlayer.ChangeHp(fightTask.Result.Damage);
                SendHp(loserClient);
                if (fightTask.Result.Tie) {
                    var winnerClient = AliveClients.Find(client => client.GamePlayer == fightTask.Result.Winner);
                    winnerClient.GamePlayer.ChangeHp(fightTask.Result.Damage);
                    SendHp(winnerClient);
                }
            }
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
                message = message + $"{componentType} ";
            }
            message = message.Remove(message.Length - 1);
            client.TcpSend(message);
        }
        
        private void SendAddShip(NetworkClient client, ShipHullName shipHullName, int newX, int newY) {
            var message = $"GAME_ADD_SHIP:{shipHullName} {newX} {newY}";
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
            var message = $"GAME_LVL_UP:{client.GamePlayer.Level}";
            client.TcpSend(message);
        }
        
        private void SendShipReposition(NetworkClient client, int shipIndex, int newX, int newY) {
            var message = $"GAME_SHIP_REPOSITION:{shipIndex} {newX} {newY}";
            client.TcpSend(message);
        }
        
        private void SendBuyComponent(NetworkClient client, int shopIndex) {
            var message = $"GAME_BUY:{shopIndex}";
            client.TcpSend(message);
        }
        
        private void SendAddBagComponent(NetworkClient client, ShipPart shipPart) {
            var message = $"GAME_ADD_COMPONENT:{shipPart.Name}";
            client.TcpSend(message);
        }

        private void AddShipComponent(Ship ship, ShipPart newShipPart) {
            ReturnToPool(ship.Parts[newShipPart.Type]);
            ship.Parts[newShipPart.Type] = newShipPart;
        }

        private void SendAddShipComponent(NetworkClient client, int shipIndex, ShipPart shipPart) {
            var message = $"GAME_ADD_SHIP_COMPONENT:{shipIndex} {shipPart.Name}";
            client.TcpSend(message);
        }

        private void AddCommandShopRoll(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_SHOP_ROLL", args => {
                if (client.GamePlayer.Money >= RollCost) {
                    client.GamePlayer.ChangeMoney(-2);
                    RollShop(client.GamePlayer);
                    SendShop(client);
                }
            });
        }

        private void AddCommandShopBuy(NetworkClient client) {
            client.AddCommand(CommandType.GAME, "GAME_SHOP_BUY", args => {
                var index = int.Parse(args[0]);
                if (client.GamePlayer.CanBuyComponent(index)) {
                    var component = client.GamePlayer.BuyComponent(index);
                    SendBuyComponent(client, index);
                    SendMoney(client);
                    client.GamePlayer.AddBagComponent(component);
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
                    player.AddXp(LvlUpCost);
                    if (player.IsNewLvl())
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
                if (player.Ships.Count < player.Level) {
                    var hullType = (ShipHullName) int.Parse(args[0]);
                    var coordinates = player.AddShip(hullType);
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
                    player.ShipReposition(player.Ships[shipIndex], newX, newY);
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