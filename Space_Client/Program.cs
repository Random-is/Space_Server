using System;

namespace Space_Client {
    internal static class Program {
        public static void Main(string[] args) {
            for (var i = 0; i < 1; i++) {
                var client = new Client("localhost", 4444);
                client.Start();
            }
            Console.ReadLine();
            // var udp = new UdpClient(4444);
            // IPEndPoint groupEP = null;
            // var bytes = udp.Receive(ref groupEP);

            // Console.WriteLine($"Received broadcast from {groupEP} :");
            // Console.WriteLine($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
        }
    }
}