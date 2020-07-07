using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Game_Elements;
using Space_Server.game;
using Space_Server.utility;

namespace Space_Server.server {
    public class Server {
        public static TcpListener TcpListener;
        public static UdpClient UdpListener;
        private readonly Random _random = new Random();
        private readonly int _port;
        public ConcurrentList<NetworkClient> Clients { get; set; }
        private int clientSize;
        public ConcurrentList<Room> Rooms { get; private set; }
        private Queue SearchQueue { get; set; }

        public Server(int port) {
            _port = port;
        }

        public void InitAndStart() {
            Clients = new ConcurrentList<NetworkClient>();
            Rooms = new ConcurrentList<Room>();
            SearchQueue = new Queue(this, 2);
            UdpListener = new UdpClient(_port);
            TcpListener = new TcpListener(IPAddress.Any, _port);
            TcpListener.Start();
            SearchQueue.Start();
            var tcpThread = new Thread(AcceptTcpClientCycle);
            tcpThread.Start();
        }

        private void AcceptTcpClientCycle() {
            while (true) {
                Log.Print("Wait for Connection");
                var client = TcpListener.AcceptTcpClient();
                var clientEndPoint = client.Client.RemoteEndPoint;
                clientSize++;
                Log.Print($"New Client Connected: {clientEndPoint} -> initialization: Start");
                var initPlayerThread = new Thread(() => InitPlayer(client, clientEndPoint, clientSize.ToString()));
                initPlayerThread.Start();
            }
        }

        private void InitPlayer(TcpClient client, EndPoint clientEndPoint, string nickname) {
            client.NoDelay = true;
            var gamePlayer = new GamePlayer();
            var networkClient = new NetworkClient(gamePlayer);
            networkClient.GenerateStreams(client);
            try {
                // networkClient.TcpReader.BaseStream.ReadTimeout = 10000;
                if (networkClient.TcpReader.ReadString() == "Hello Space Comrade") {
                    var authMessage = networkClient.TcpReader.ReadString(); // login:password
                    var auth = authMessage.Split(':');
                    var login = auth[0];
                    var password = auth[1];
                    //todo Get Nickname from BD by [login, password]
                    gamePlayer.Nickname = nickname;
                    // networkClient.TcpReader.BaseStream.ReadTimeout = -1;
                    Clients.TryAdd(networkClient);
                    AddCommandHandler(networkClient);
                    AddDisconnectHandler(networkClient);
                    networkClient.StartCommandHandler();
                    networkClient.TcpSend(gamePlayer.Nickname);
                }
            } catch (Exception) {
                client.Close();
                Log.Print($"Client Disconnected: {clientEndPoint} -> initialization: Cancel");
            }
            Log.Print($"Client initialization Complete: {clientEndPoint} -> {gamePlayer.Nickname}");
        }

        private void AddCommandHandler(NetworkClient client) {
            client.AddCommand(CommandType.SERVER, "QUEUE_ENTER", args => SearchQueue.Join(client));
            client.AddCommand(CommandType.SERVER, "QUEUE_LEAVE", args => SearchQueue.Leave(client));
        }

        private void RemoveCommandHandler(NetworkClient client) {
            client.RemoveAllCommands(CommandType.SERVER);
        }

        private void AddDisconnectHandler(NetworkClient client) {
            client.AddDisconnectHandler(CommandType.SERVER, () => {
                if (Clients.TryRemove(client))
                    Log.Print($"{client.GamePlayer.Nickname} (Server) deleted from Clients");
            });
        }

        private void RemoveDisconnectHandler(NetworkClient client) {
            client.RemoveDisconnectHandler(CommandType.SERVER);
        }

        private string GenerateNickname() {
            var randomWord = new char[14];
            for (var i = 0; i < randomWord.Length; i++)
                randomWord[i] = (char) ((i & 1) == 0 ? _random.Next(65, 91) : _random.Next(48, 58));
            return new string(randomWord);
        }
    }
}