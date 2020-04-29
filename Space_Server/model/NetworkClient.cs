using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Space_Server.model {
    public class NetworkClient {
        public TcpClient TcpClient { get; }

        public BinaryWriter TcpWriter { get; set; }

        public BinaryReader TcpReader { get; set; }

        public GamePlayer GamePlayer { get; }

        public NetworkClient(TcpClient tcpClient, GamePlayer gamePlayer) {
            TcpClient = tcpClient;
            GamePlayer = gamePlayer;
        }

        public void GenerateStreams(TcpClient tcpClient) {
            TcpWriter = new BinaryWriter(tcpClient.GetStream());
            TcpReader = new BinaryReader(tcpClient.GetStream());
        }

        public ConcurrentDictionary<CommandType, ConcurrentDictionary<string, Action<string[]>>> CommandHandler { get; }
            = new ConcurrentDictionary<CommandType, ConcurrentDictionary<string, Action<string[]>>>();

        public ConcurrentDictionary<CommandType, Action> DisconnectHandler { get; } =
            new ConcurrentDictionary<CommandType, Action>();

        public void StartCommandHandler() {
            var commandHandlerThread = new Thread(CommandHandlerCycle);
            commandHandlerThread.Start();
        }

        public void TcpSend(string message) {
            try {
                TcpWriter.Write(message);
            } catch (Exception) {
                Disconnect();
            }
        }

        private void CommandHandlerCycle() {
            try {
                while (true) {
                    var message = TcpReader.ReadString();
                    Log.Print($"{GamePlayer.Nickname} -> [{message}]");
                    var command = message.Split(':');
                    var args = command.Length > 1 ? command[1].Split(' ') : default;
                    foreach (var commands in CommandHandler.Values)
                        if (commands.TryGetValue(command[0], out var action)) {
                            action(args);
                            break;
                        }
                }
            } catch (Exception) {
                Disconnect();
            }
        }

        private void Disconnect() {
            Log.Print($"Disconnect: {GamePlayer.Nickname} -> Start");
            foreach (var action in DisconnectHandler.Values) action();
            Log.Print($"Disconnect: {GamePlayer.Nickname} -> Complete");
        }


        public void AddCommand(CommandType commandSource, string command, Action<string[]> action) {
            if (CommandHandler.TryGetValue(commandSource, out var dict)) {
                dict.TryAdd(command, action);
            } else {
                CommandHandler.TryAdd(commandSource, new ConcurrentDictionary<string, Action<string[]>> {
                    [command] = action
                });
            }
        }

        public void RemoveCommand(CommandType commandSource, string command) {
            if (CommandHandler.TryGetValue(commandSource, out var dict)) {
                dict.TryRemove(command, out _);
            }
        }

        public void RemoveAllCommands(CommandType commandSource) {
            CommandHandler.TryRemove(commandSource, out _);
        }

        public void AddDisconnectHandler(CommandType disconnectSource, Action action) {
            DisconnectHandler.TryAdd(disconnectSource, action);
        }

        public void RemoveDisconnectHandler(CommandType disconnectSource) {
            DisconnectHandler.TryRemove(disconnectSource, out _);
        }
    }
}