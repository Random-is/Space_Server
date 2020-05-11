using System;
using System.Runtime.CompilerServices;

namespace Space_Server.utility {
    internal static class Log {
        public static void Print(string message) {
            Console.Write($"[{DateTime.Now}] {message}\n");
        }

        public static void Debug(string message, [CallerMemberName] string callerName = "") {
            Print($"{callerName}: {message}");
        }
    }
}