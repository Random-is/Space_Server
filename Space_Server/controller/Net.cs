using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Space_Server.model;

namespace Space_Server.controller {
    internal static class Net {
        public static void SendAll(List<NetworkClient> players, string message) {
            players.ForEach(player => player.TcpSend(message));
        }

        public static ConcurrentDictionary<NetworkClient, bool> WaitAllAsync(List<NetworkClient> clients, string message) {
            return AddTempCommandAll(clients, message);
        }

        public static bool WaitAll(
            List<NetworkClient> clients,
            string message,
            out ConcurrentDictionary<NetworkClient, bool> handled,
            Action beforeWait = null,
            int timeout = -1
        ) {
            var locker = new CountdownEvent(clients.Count);
            handled = AddTempCommandAll(clients, message, client => locker.Signal());
            beforeWait?.Invoke();
            var result = locker.Wait(timeout);
            RemoveTempCommandAll(clients);
            locker.Dispose();
            return result;
        }

        private static ConcurrentDictionary<NetworkClient, bool> AddTempCommandAll(
            IReadOnlyCollection<NetworkClient> clients,
            string message,
            Action<NetworkClient> action = null
        ) {
            var handled = new ConcurrentDictionary<NetworkClient, bool>();
            foreach (var client in clients) {
                if (!handled.TryAdd(client, false)) {
                    Console.WriteLine("EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE " + client.Player.Nickname);
                }
            }
            
            foreach (var client in clients) {
                client.CommandHandler.Add(CommandType.TEMP, new Dictionary<string, Action<string[]>> {
                    [message] = args => {
                        client.CommandHandler.Remove(CommandType.TEMP);
                        handled[client] = true;
                        action?.Invoke(client);
                    }
                });
            }
            return handled;
        }

        private static void RemoveTempCommandAll(IEnumerable<NetworkClient> clients) {
            foreach (var client in clients)
                client.CommandHandler.Remove(CommandType.TEMP);
        }
    }
}