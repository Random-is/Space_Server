using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Space_Server.model;

namespace Space_Server.controller {
    public class Queue : List<NetworkPlayer> {
        private readonly object _addRemoveLocker = new object();
        private readonly List<Room> _rooms;
        private readonly int _roomSize;
        private readonly Server _server;
        private readonly Dictionary<NetworkPlayer, bool> _requestAcceptPlayers = new Dictionary<NetworkPlayer, bool>();
        private readonly object dictionaryLocker = new object();

        public Queue(Server server, int roomSize) {
            _rooms = server.Rooms;
            _server = server;
            _roomSize = roomSize;
        }

        public void Start() {
            var searchPartyThread = new Thread(SearchPartyCycle);
            searchPartyThread.Start();
        }


        private bool TryPop(out NetworkPlayer player) {
            try {
                player = this[0];
                RemoveAt(0);
                return true;
            } catch (Exception) {
                player = default;
                return false;
            }
        }

        private bool TryTakePlayersToList(ICollection<NetworkPlayer> players) {
            for (var i = 0; i < _roomSize; i++) {
                if (!TryPop(out var player)) {
                    AddRange(players);
                    return false;
                }

                players.Add(player);
            }

            return true;
        }

        public void AcceptGame(NetworkPlayer player) {
            if (_requestAcceptPlayers.ContainsKey(player)) {
                _requestAcceptPlayers[player] = true;
            }
        }

        private void RemoveFromRequest(NetworkPlayer player) {
            lock (dictionaryLocker) {
                _requestAcceptPlayers.Remove(player);
            }
        }

        private void AddRequest(NetworkPlayer player) {
            lock (dictionaryLocker) {
                _requestAcceptPlayers.Add(player, false);
            }
        }

        private void AcceptRoomCreation(List<NetworkPlayer> players) {
            players.ForEach(AddRequest);
            Net.SendAll(players, "QUEUE FOUND");
            const int timeToAccept = 20;
            for (var i = 0; i < timeToAccept; i++) {
                Log.Print($"Time to Accept {timeToAccept - i}");
                Log.Print($"Already Accepted: {players.Count(player => _requestAcceptPlayers[player])}");
                Thread.Sleep(1000);
                if (players.Count(player => _requestAcceptPlayers[player]) == players.Count) {
                    Log.Print("Create new Room");
                    players.ForEach(RemoveFromRequest);
                    Net.SendAll(players, "QUEUE LEAVED");
                    var room = new Room(players);
                    _rooms.Add(room);
                    room.Start();
                    return;
                }
            }
            foreach (var player in players) {
                if (_requestAcceptPlayers[player])
                    Add(player);
                else
                    player.TcpSend("QUEUE LEAVED");
                _requestAcceptPlayers.Remove(player);
            }
        }

        public void Join(NetworkPlayer player) {
            lock (_addRemoveLocker) {
                player.TcpSend("QUEUE JOINED");
                Add(player);
            }
        }

        public void Leave(NetworkPlayer player) {
            lock (_addRemoveLocker) {
                Remove(player);
                player.TcpSend("QUEUE LEAVED");
            }
        }

        public void Disconnect(NetworkPlayer player) {
            if (Remove(player)) {
                Log.Print($"{player.Player.Nickname} disconnect from Queue");
            }
        }

        private void SearchPartyCycle() {
            while (true) {
                Log.Print($"Queue Size: {Count.ToString()}");
                Log.Print($"Dictionary Size: {_requestAcceptPlayers.Count}");
                if (Count >= _roomSize) {
                    var canCreate = Count / _roomSize;
                    for (var i = 0; i < canCreate; i++) {
                        var players = new List<NetworkPlayer>();
                        if (TryTakePlayersToList(players)) {
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