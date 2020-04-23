using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Space_Server.model;

namespace Space_Server.controller {
    public class Queue : List<NetworkClient> {
        private readonly object _loinLeaveLocker = new object();
        private readonly List<Room> _rooms;
        private readonly int _roomSize;
        private readonly Server _server;

        public Queue(Server server, int roomSize) {
            _rooms = server.Rooms;
            _server = server;
            _roomSize = roomSize;
        }

        public void Start() {
            var searchPartyThread = new Thread(SearchPartyCycle);
            searchPartyThread.Start();
        }


        private bool TryPop(out NetworkClient client) {
            try {
                client = this[0];
                RemoveAt(0);
                return true;
            } catch (Exception) {
                client = default;
                return false;
            }
        }

        private bool TryTakePlayersToList(ICollection<NetworkClient> players) {
            for (var i = 0; i < _roomSize; i++) {
                if (!TryPop(out var player)) {
                    AddRange(players);
                    return false;
                }
                players.Add(player);
            }
            return true;
        }

        private void AcceptRoomCreation(List<NetworkClient> clients) {
            const int timeToAccept = 20;
            Log.Print($"Time to Accept {timeToAccept} sec");
            if (Net.WaitAll(clients, "QUEUE_ACCEPT", out var handled, () => Net.SendAll(clients, "QUEUE FOUND"),
                timeToAccept * 1000)) {
                Log.Print("Create new Room");
                Net.SendAll(clients, "QUEUE_LEAVE");
                var room = new Room(clients);
                _rooms.Add(room);
                room.Start();
            } else {
                foreach (var client in handled.Keys) {
                    if (handled[client])
                        lock (_loinLeaveLocker) {
                            Add(client);
                        }
                    else
                        Leave(client);
                }
            }
        }

        private void AcceptRoomCreationOld(List<NetworkClient> players) {
            var handled = Net.WaitAllAsync(players, "QUEUE_ACCEPT");
            Net.SendAll(players, "QUEUE FOUND");
            const int timeToAccept = 20;
            for (var i = 0; i < timeToAccept; i++) {
                Log.Print($"Time to Accept {timeToAccept - i}");
                Log.Print($"Already Accepted: {handled.Values.Count(value => value)}");
                Thread.Sleep(1000);
                if (handled.Values.Count(value => value) == players.Count) {
                    Log.Print("Create new Room");
                    Net.SendAll(players, "QUEUE_LEAVE");
                    var room = new Room(players);
                    _rooms.Add(room);
                    room.Start();
                    return;
                }
            }
            foreach (var player in players) {
                if (handled[player])
                    Add(player);
                else
                    player.TcpSend("QUEUE_LEAVE");
            }
        }

        public void Join(NetworkClient client) {
            lock (_loinLeaveLocker) {
                AddDisconnectHandler(client);
                Add(client);
                client.TcpSend("QUEUE_JOIN");
            }
        }

        public void Leave(NetworkClient client) {
            lock (_loinLeaveLocker) {
                RemoveDisconnectHandler(client);
                Remove(client);
                client.TcpSend("QUEUE_LEAVE");
            }
        }
        
        private void AddDisconnectHandler(NetworkClient client) {
            client.DisconnectHandler.Add(CommandType.QUEUE, () => {
                lock (_loinLeaveLocker) {
                    if (Remove(client)) {
                        Log.Print($"{client.Player.Nickname} (Queue) deleted from Queue");
                    }
                }
            });
        }

        private void RemoveDisconnectHandler(NetworkClient client) {
            client.DisconnectHandler.Remove(CommandType.QUEUE);
        }


        private void SearchPartyCycle() {
            while (true) {
                Log.Print($"Queue Size: {Count.ToString()}");
                // if (Count >= _roomSize) {
                //     var canCreate = Count / _roomSize;
                //     for (var i = 0; i < canCreate; i++) {
                //         var players = new List<NetworkClient>();
                //         if (TryTakePlayersToList(players)) {
                //             var roomCreationThread = new Thread(() => AcceptRoomCreation(players));
                //             roomCreationThread.Start();
                //         }
                //     }
                // }
                Thread.Sleep(2000);
            }
        }
    }
}