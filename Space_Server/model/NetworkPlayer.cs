using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Space_Server.model {
    public class NetworkPlayer {
        public NetworkPlayer(TcpClient tcpClient, GamePlayer player) {
            TcpClient = tcpClient;
            Player = player;
            TcpWriter = new BinaryWriter(tcpClient.GetStream());
            TcpReader = new BinaryReader(tcpClient.GetStream());
        }

        public TcpClient TcpClient { get; }
        public BinaryWriter TcpWriter { get; }
        public BinaryReader TcpReader { get; }
        public GamePlayer Player { get; }

        public IClientCommandHandler CommandHandler { get; set; }
        
        public Dictionary<string, Action<string[]>> CommandHandlerDictionary { get; } = new Dictionary<string, Action<string[]>>();
        
        public IClientDisconnectHandler DisconnectHandler { get; set; }

        public void StartCommandHandler() {
            var commandHandlerThread = new Thread(CommandHandlerCycle);
            commandHandlerThread.Start();
        }

        private void CommandHandlerCycle() {
            try {
                while (true) {
                    var message = TcpReader.ReadString();
                    var command = message.Split(':');
                    Console.WriteLine("asdasd " + command.Length);
                    foreach (var t in command) {
                        Console.WriteLine("asdasdasdasd " + t);
                    }
                    Console.WriteLine("asdasd " + command.Length);
                    Log.Print($"{Player.Nickname} -> [{message}]");
                    CommandHandlerDictionary[message](command);
                    // CommandHandler?.Handle(this, command);
                }
            } catch (Exception) {
                DisconnectHandler.Disconnect(this);
            }
        }

        public void TcpSend(string message) {
            try {
                TcpWriter.Write(message);
            } catch (Exception) {
                DisconnectHandler.Disconnect(this);
            }
        }
        
        public void TcpSend(int message) {
            try {
                TcpWriter.Write(message);
            } catch (Exception) {
                DisconnectHandler.Disconnect(this);
            }
        }
    }
}