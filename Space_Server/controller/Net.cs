using System.Collections.Generic;
using Space_Server.model;

namespace Space_Server.controller {
    internal static class Net {
        public static void SendAll(List<NetworkPlayer> players, string message) {
            players.ForEach(player => player.TcpSend(message));
        }

        public static void SendAll(List<NetworkPlayer> players, int message) {
            players.ForEach(player => player.TcpSend(message));
        }

        public static void WaitAll(List<NetworkPlayer> players, string message) {
            
        }
    }
}