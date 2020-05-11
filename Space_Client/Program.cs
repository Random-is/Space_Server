using System;
using System.Collections.Generic;
using System.Linq;
using Space_Client.model;

namespace Space_Client {
    internal static class Program {
        public static void Main(string[] args) {
            for (var i = 0; i < 1; i++) {
                var client = new Client("localhost", 4444);
                client.Start();
            }
            Console.ReadLine();
            // const int length = 4;
            // var random = new Random();
            // for (var j = 0; j < 1000; j++) {
            //     var indexes = new int[length];
            //     var range = Enumerable.Range(0, indexes.Length).ToList();
            //     for (var i = 0; i < indexes.Length; i++) {
            //         if (range.Count == 2) {
            //             var index = range.IndexOf(indexes.Length - 1);
            //             if (index != -1) {
            //                 indexes[i] = range[index];
            //                 indexes[i + 1] = range[index == 0 ? 1 : 0];
            //                 break;
            //             }
            //         }
            //         var randomIndex = random.Next(range.Count);
            //         while (range[randomIndex] == i) {
            //             randomIndex = random.Next(range.Count);
            //         }
            //         indexes[i] = range[randomIndex];
            //         range.RemoveAt(randomIndex);
            //     }
            //     
            //     Console.Write("[");
            //     foreach (var i in indexes) {
            //         Console.Write(i + " ");
            //     }
            //     Console.Write("]");
            //     Console.WriteLine();
        }
    }
}