using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Space_Server.model {
    public class NetworkClient {
        public NetworkClient(TcpClient tcpClient, GamePlayer player) {
            TcpClient = tcpClient;
            Player = player;
            TcpWriter = new BinaryWriter(tcpClient.GetStream());
            TcpReader = new BinaryReader(tcpClient.GetStream());
        }

        public TcpClient TcpClient { get; }
        public BinaryWriter TcpWriter { get; }
        public BinaryReader TcpReader { get; }
        public GamePlayer Player { get; }
        public Dictionary<CommandType, Dictionary<string, Action<string[]>>> CommandHandler { get; } = new Dictionary<CommandType, Dictionary<string, Action<string[]>>>();
        public Dictionary<CommandType, Action> DisconnectHandler { get; } = new Dictionary<CommandType, Action>();
        public void StartCommandHandler() {
            var commandHandlerThread = new Thread(CommandHandlerCycle);
            commandHandlerThread.Start();
        }

        private void Disconnect() {
            Log.Print($"Disconnect: {Player.Nickname} -> Start");
            foreach (var action in DisconnectHandler.Values) {
                action();
            }
            Log.Print($"Disconnect: {Player.Nickname} -> Complete");
        }

        private void CommandHandlerCycle() {
            try {
                while (true) {
                    var message = TcpReader.ReadString();
                    Log.Print($"{Player.Nickname} -> [{message}]");
                    var command = message.Split(':');
                    var args = command.Length > 1 ? command[1].Split(' ') : default;
                    foreach (var commands in CommandHandler.Values) {
                        if (commands.TryGetValue(command[0], out var action)) {
                            action(args);
                            break;
                        }
                    }
                }
            } catch (Exception) {
                Disconnect();
            }
        }

        public void TcpSend(string message) {
            try {
                TcpWriter.Write(message);
            } catch (Exception) {
                Disconnect();
            }
        }
        
        public void TcpSend(int message) {
            try {
                TcpWriter.Write(message);
            } catch (Exception) {
                Disconnect();
            }
        }
    }
}