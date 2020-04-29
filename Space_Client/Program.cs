using System;
using Space_Client.model;

namespace Space_Client {
    internal static class Program {
        public static void Main(string[] args) {
            for (var i = 0; i < 1; i++) {
                var client = new Client("localhost", 4444);
                client.Start();
            }
            Console.ReadLine();
        }
    }
}