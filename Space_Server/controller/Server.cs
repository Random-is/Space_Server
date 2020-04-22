﻿﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Space_Server.model;

namespace Space_Server.controller {
    public class Server {
        public static TcpListener TcpListener;
        public static UdpClient UdpListener;
        private static readonly Random random = new Random();
        private readonly int _port;

        public List<NetworkPlayer> Players { get; set; }

        public List<Room> Rooms { get; set; }

        public Queue SearchQueue { get; set; }

        public Server(int port) {
            _port = port;
        }

        public void Start() {
            Players = new List<NetworkPlayer>();
            Rooms = new List<Room>();
            SearchQueue = new Queue(this, 4);
            SearchQueue.Start();
            UdpListener = new UdpClient(_port);
            TcpListener = new TcpListener(IPAddress.Any, _port);
            TcpListener.Start();
            var tcpThread = new Thread(AcceptTcpClientCycle);
            tcpThread.Start();
        }

        private void AcceptTcpClientCycle() {
            while (true) {
                Log.Print("Wait for new Connection");
                var client = TcpListener.AcceptTcpClient();
                Log.Print($"New Client Connected: {client.Client.RemoteEndPoint} -> initialization: Start");
                var initPlayerThread = new Thread(() => InitPlayer(client));
                initPlayerThread.Start();
            }
        }

        private void InitPlayer(TcpClient client) {
            var gamePlayer = new GamePlayer();
            var networkPlayer = new NetworkPlayer(client, gamePlayer);
            try {
                networkPlayer.TcpReader.BaseStream.ReadTimeout = 10000;
                if (networkPlayer.TcpReader.ReadString() == "Hello Space Comrade") {
                    var authMessage = networkPlayer.TcpReader.ReadString(); // login:password
                    var auth = authMessage.Split(':');
                    var login = auth[0];
                    var password = auth[1];
                    //todo Get Nickname from BD by [login, password]
                    // gamePlayer.Nickname = GenerateNickname();
                    gamePlayer.Nickname = Players.Count.ToString();
                    networkPlayer.TcpReader.BaseStream.ReadTimeout = -1;
                    Players.Add(networkPlayer);
                    networkPlayer.DisconnectHandler = new InServerDisconnectHandler(this);
                    networkPlayer.CommandHandler = new InServerClientCommandHandler(this);
                    networkPlayer.CommandHandlerDictionary.Add("QUEUE ENTER", args => SearchQueue.Join(networkPlayer));
                    networkPlayer.StartCommandHandler();
                    networkPlayer.TcpSend(gamePlayer.Nickname);
                }
            } catch (Exception e) {
                Log.Print(e.ToString());
                client.Close();
                Log.Print($"Client Disconnected: {client.Client.RemoteEndPoint} -> initialization: Cancel");
            }

            Log.Print($"Client initialization Complete: {client.Client.RemoteEndPoint} -> {gamePlayer.Nickname}");
        }

        private static string GenerateNickname() {
            var randomWord = new char[14];
            for (var i = 0; i < randomWord.Length; i++)
                randomWord[i] = (char) ((i & 1) == 0 ? random.Next(65, 91) : random.Next(48, 58));
            return new string(randomWord);
        }

        private class InServerClientCommandHandler : IClientCommandHandler {
            private readonly Server _server;

            public InServerClientCommandHandler(Server server) {
                _server = server;
            }

            public void Handle(NetworkPlayer player, string[] command) {
                if (command[0] == "QUEUE") {
                    if (command[1] == "ENTER")
                        _server.SearchQueue.Join(player);
                    else if (command[1] == "LEAVE")
                        _server.SearchQueue.Leave(player);
                    else if (command[1] == "ACCEPT") _server.SearchQueue.AcceptGame(player);
                }
            }
        }
        
        private class InServerDisconnectHandler : IClientDisconnectHandler {
            private readonly Server _server;

            public InServerDisconnectHandler(Server server) {
                _server = server;
            }
            public void Disconnect(NetworkPlayer player) {
                Log.Print($"Disconnect: {player.Player.Nickname} -> Start");
                _server.SearchQueue.Disconnect(player);
                _server.Disconnect(player);
                Log.Print($"Disconnect: {player.Player.Nickname} -> Complete");
            }
        }

        private void Disconnect(NetworkPlayer player) {
            if (Players.Remove(player)) {
                Log.Print($"{player.Player.Nickname} disconnect from Server");
            }
        }
    }
}