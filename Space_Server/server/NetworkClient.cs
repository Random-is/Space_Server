using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using Space_Server.game;
using Space_Server.utility;

namespace Space_Server.server {
    public class NetworkClient {
        public bool Connected { get; set; }
        public TcpClient TcpClient { get; set; }
        public BinaryWriter TcpWriter { get; set; }
        public BinaryReader TcpReader { get; set; }
        public GamePlayer GamePlayer { get; }
        public ConcurrentDictionary<CommandType, ConcurrentDictionary<string, Action<string[]>>> CommandHandler { get; }
            = new ConcurrentDictionary<CommandType, ConcurrentDictionary<string, Action<string[]>>>();
        public ConcurrentDictionary<CommandType, Action> DisconnectHandler { get; } 
            = new ConcurrentDictionary<CommandType, Action>();

        public NetworkClient(GamePlayer gamePlayer) {
            GamePlayer = gamePlayer;
        }

        public void GenerateStreams(TcpClient tcpClient) {
            TcpClient = tcpClient;
            TcpWriter = new BinaryWriter(tcpClient.GetStream());
            TcpReader = new BinaryReader(tcpClient.GetStream());
            Connected = true;
        }

        public void TcpSend(string message) {
            if (Connected) {
                try {
                    TcpWriter.Write(message);
                    Log.Print($"TCP send [{message}] -> {GamePlayer.Nickname}");
                } catch (Exception) {
                    Disconnect();
                } 
            }
        }

        public void StartCommandHandler() {
            var commandHandlerThread = new Thread(CommandHandlerCycle);
            commandHandlerThread.Start();
        }

        private void CommandHandlerCycle() {
            try {
                while (true) {
                    var message = TcpReader.ReadString();
                    Log.Print($"TCP in [{message}] -> {GamePlayer.Nickname}");
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
            if (Connected) {
                Connected = false;
                Log.Print($"Disconnect START -> {GamePlayer.Nickname}");
                foreach (var action in DisconnectHandler.Values) action();
                Log.Print($"Disconnect COMPLETE -> {GamePlayer.Nickname}");
            }
        }


        public void AddCommand(CommandType commandSource, string command, Action<string[]> action) {
            if (CommandHandler.TryGetValue(commandSource, out var dict))
                dict.TryAdd(command, action);
            else
                CommandHandler.TryAdd(commandSource, new ConcurrentDictionary<string, Action<string[]>> {
                    [command] = action
                });
        }

        public void RemoveCommand(CommandType commandSource, string command) {
            if (CommandHandler.TryGetValue(commandSource, out var dict)) dict.TryRemove(command, out _);
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