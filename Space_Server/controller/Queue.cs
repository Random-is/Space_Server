using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Space_Server.model;

namespace Space_Server.controller {
    public class Queue : ConcurrentList<NetworkClient> {
        private readonly object _loinLeaveLocker = new object();
        private readonly ConcurrentList<Room> _rooms;
        private readonly int _roomSize;
        private readonly Server _server;
        private int roomFailed = 0;
        private int playersLeave = 0;

        public Queue(Server server, int roomSize) {
            _rooms = server.Rooms;
            _server = server;
            _roomSize = roomSize;
        }

        public void Start() {
            var searchPartyThread = new Thread(SearchPartyCycle);
            searchPartyThread.Start();
        }

        private bool TryTakePlayersToList(ICollection<NetworkClient> players, int count) {
            for (var i = 0; i < count; i++) {
                if (!TryPop(out var player)) {
                    TryAddRange(players);
                    return false;
                }
                players.Add(player);
            }
            return true;
        }

        private void AcceptRoomCreation(ConcurrentList<NetworkClient> clients) {
            const int timeToAccept = 20;
            Log.Print($"Time to Accept {timeToAccept} sec");
            if (Net.WaitAll(clients, "QUEUE_ACCEPT", out var handled, () => Net.SendAll(clients, "QUEUE_FOUND"))) {
                Log.Print($"Create new Room {_rooms.Count}");
                Net.SendAll(clients, "QUEUE_LEAVE");
                var room = new Room(clients);
                _rooms.TryAdd(room);
                room.Start();
            } else {
                roomFailed++;
                foreach (var client in handled.Keys)
                    if (handled[client]) {
                        TryAdd(client);
                        client.TcpSend("QUEUE_CONTINUE");
                    } else {
                        playersLeave++;
                        Leave(client);
                    }
            }
        }

        public void Join(NetworkClient client) {
            AddDisconnectHandler(client);
            TryAdd(client);
            client.TcpSend("QUEUE_JOIN");
        }

        public void Leave(NetworkClient client) {
            RemoveDisconnectHandler(client);
            TryRemove(client);
            client.TcpSend("QUEUE_LEAVE");
        }

        private void AddDisconnectHandler(NetworkClient client) {
            client.AddDisconnectHandler(CommandType.QUEUE, () => {
                if (TryRemove(client)) 
                    Log.Print($"{client.GamePlayer.Nickname} (Queue) deleted from Queue");
            });
        }

        private void RemoveDisconnectHandler(NetworkClient client) {
            client.RemoveDisconnectHandler(CommandType.QUEUE);
        }


        private void SearchPartyCycle() {
            while (true) {
                Log.Print($"Queue Size: {Count}");
                Log.Print($"Rooms Count: {_rooms.Count}");
                Log.Print($"Clients Count: {_server.Clients.Count}");
                if (Count >= _roomSize) {
                    var canCreate = Count / _roomSize;
                    for (var i = 0; i < canCreate; i++) {
                        var players = new ConcurrentList<NetworkClient>();
                        if (TryTakePlayersToList(players, _roomSize)) {
                            var roomCreationThread = new Thread(() => AcceptRoomCreation(players));
                            roomCreationThread.Start();
                        }
                    }
                }
                Thread.Sleep(2000);
            }
        }
    }
}