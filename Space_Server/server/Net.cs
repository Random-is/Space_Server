using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Space_Server.server {
    internal static class Net {
        public static void SendAll(List<NetworkClient> players, string message) {
            players.ForEach(player => player.TcpSend(message));
        }

        public static ConcurrentDictionary<NetworkClient, bool> WaitAllAsync(
            IEnumerable<NetworkClient> clients,
            string message
        ) {
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
            return result;
        }

        private static ConcurrentDictionary<NetworkClient, bool> AddTempCommandAll(
            IEnumerable<NetworkClient> clients,
            string message,
            Action<NetworkClient> action = null
        ) {
            var handled = new ConcurrentDictionary<NetworkClient, bool>();
            foreach (var client in clients) {
                handled.TryAdd(client, false);
                client.CommandHandler.TryAdd(CommandType.TEMP, new ConcurrentDictionary<string, Action<string[]>> {
                    [message] = args => {
                        client.CommandHandler.TryRemove(CommandType.TEMP, out _);
                        handled[client] = true;
                        action?.Invoke(client);
                    }
                });
            }
            return handled;
        }

        private static void RemoveTempCommandAll(IEnumerable<NetworkClient> clients) {
            foreach (var client in clients)
                client.RemoveAllCommands(CommandType.TEMP);
        }
    }
}