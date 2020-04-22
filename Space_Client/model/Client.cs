﻿using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Space_Client {
    public class Client {
        private readonly string _ip;
        private readonly int _port;

        public Client(string ip, int port) {
            _ip = ip;
            _port = port;
        }

        public void Start() {
            Console.WriteLine("Подключаюсь к серверу");
            var client = new TcpClient(_ip, _port);
            Console.WriteLine("Подключился");
            var binaryWriter = new BinaryWriter(client.GetStream());
            var binaryReader = new BinaryReader(client.GetStream());
            binaryWriter.Write("Hello Space Comrade");
            binaryWriter.Write("Login:Password");
            var nickname = binaryReader.ReadString();
            Console.WriteLine($"My new Nickname: {nickname}");
            binaryWriter.Write("QUEUE ENTER:");
            var listenMessageThread = new Thread(() => {
                while (true) binaryReader.ReadString();
            });
            listenMessageThread.Start();
            // Console.ReadLine();
            // binaryWriter.Write("QUEUE ACCEPT");
            // Console.ReadLine();
        }
    }
}