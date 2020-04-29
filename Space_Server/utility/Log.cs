using System;

namespace Space_Server.utility {
    internal static class Log {
        public static void Print(string message) {
            Console.Write($"[{DateTime.Now}] {message}\n");
        }
    }
}