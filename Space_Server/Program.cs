using System;
using System.Collections.Generic;
using Space_Server.controller;

namespace Space_Server {
    internal static class Program {
        public static void Main(string[] args) {
            GC.GetTotalMemory(false);
            var server = new Server(4444);
            server.Start();
            while (true) {
                Console.ReadLine();
                Log.Print("GC Collect -> Start");
                GC.Collect(2, GCCollectionMode.Forced);
                Log.Print("GC Collect -> Complete");
            }
        }
    }
}