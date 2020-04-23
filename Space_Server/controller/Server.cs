﻿using System;
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

        public List<NetworkClient> Clients { get; set; }

        public List<Room> Rooms { get; set; }

        public Queue SearchQueue { get; set; }

        public Server(int port) {
            _port = port;
        }

        public void Start() {
            Clients = new List<NetworkClient>();
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
            var networkClient = new NetworkClient(client, gamePlayer);
            try {
                networkClient.TcpReader.BaseStream.ReadTimeout = 10000;
                if (networkClient.TcpReader.ReadString() == "Hello Space Comrade") {
                    var authMessage = networkClient.TcpReader.ReadString(); // login:password
                    var auth = authMessage.Split(':');
                    var login = auth[0];
                    var password = auth[1];
                    //todo Get Nickname from BD by [login, password]
                    gamePlayer.Nickname = Clients.Count.ToString();
                    networkClient.TcpReader.BaseStream.ReadTimeout = -1;
                    Clients.Add(networkClient);
                    AddCommandHandler(networkClient);
                    AddDisconnectHandler(networkClient);
                    networkClient.StartCommandHandler();
                    networkClient.TcpSend(gamePlayer.Nickname);
                }
            } catch (Exception e) {
                Log.Print(e.ToString());
                client.Close();
                Log.Print($"Client Disconnected: {client.Client.RemoteEndPoint} -> initialization: Cancel");
            }

            Log.Print($"Client initialization Complete: {client.Client.RemoteEndPoint} -> {gamePlayer.Nickname}");
        }

        private void AddCommandHandler(NetworkClient client) {
            client.CommandHandler.Add(CommandType.SERVER, new Dictionary<string, Action<string[]>> {
                ["QUEUE_ENTER"] = args => SearchQueue.Join(client),
                ["QUEUE_LEAVE"] = args => SearchQueue.Leave(client),
            });
        }

        private void RemoveCommandHandler(NetworkClient client) {
            client.CommandHandler.Remove(CommandType.SERVER);
        }

        private void AddDisconnectHandler(NetworkClient client) {
            client.DisconnectHandler.Add(CommandType.SERVER, () => {
                if (Clients.Remove(client)) {
                    Log.Print($"{client.Player.Nickname} (Server) deleted from Clients");
                }
            });
        }

        private void RemoveDisconnectHandler(NetworkClient client) {
            client.DisconnectHandler.Remove(CommandType.SERVER);
        }

        private static string GenerateNickname() {
            var randomWord = new char[14];
            for (var i = 0; i < randomWord.Length; i++)
                randomWord[i] = (char) ((i & 1) == 0 ? random.Next(65, 91) : random.Next(48, 58));
            return new string(randomWord);
        }
    }
}