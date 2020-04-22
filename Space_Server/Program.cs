using System;
using Space_Server.controller;

namespace Space_Server {
    internal static class Program {
        public static void Main(string[] args) {
            var server = new Server(4444);
            server.Start();
            while (true) {
                Console.ReadLine();
                Log.Print("GC Collect -> Start");
                GC.Collect();
                Log.Print("GC Collect -> Complete");
            }
            
            // var udp = new UdpClient(4443);
            // var sendbuf = Encoding.ASCII.GetBytes("Hello");
            // udp.Connect("localhost", 4444);
            // udp.Send(sendbuf, sendbuf.Length);
        }
    }
}