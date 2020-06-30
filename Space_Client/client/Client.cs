using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Space_Client.model {
    public class Client {
        private readonly string _ip;
        private readonly int _port;

        public Client(string ip, int port) {
            _ip = ip;
            _port = port;
        }

        public void Start() {
            // var t = new Thread(() => {
                Console.WriteLine("Подключаюсь к серверу");
                TcpClient client;
                while (true)
                    try {
                        client = new TcpClient(_ip, _port);
                        break;
                    } catch (Exception) {
                        Console.WriteLine("Повторное подключениееееееееееееееее");
                    }
                Console.WriteLine("Подключился");
                var binaryWriter = new BinaryWriter(client.GetStream());
                var binaryReader = new BinaryReader(client.GetStream());
                binaryWriter.Write("Hello Space Comrade");
                binaryWriter.Flush();
                binaryWriter.Write("Login:Password");
                binaryWriter.Flush();
                var nickname = binaryReader.ReadString();
                Console.WriteLine($"My new Nickname: {nickname}");
                binaryWriter.Write("QUEUE_ENTER");
                binaryWriter.Flush();
                var listenMessageThread = new Thread(() => {
                    while (true) {
                        var message = binaryReader.ReadString();
                        Console.WriteLine(message);
                        if (message == "QUEUE_FOUND")
                        binaryWriter.Write("QUEUE_ACCEPT");
                        else if (message == "ROOM_CREATED") binaryWriter.Write("ROOM_LOADED");
                    }
                });
                listenMessageThread.Start();
                // while (true) {
                //     Console.ReadLine();
                //     binaryWriter.Write("QUEUE_ACCEPT");
                // }
            // });
            // t.Start();

            // Console.ReadLine();
            // binaryWriter.Write("QUEUE_ACCEPT");
            // Console.ReadLine();
        }
    }
}