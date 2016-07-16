using System;

namespace InvertedTomato.Feather.TestServer {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Starting server...");
            using (var server = Feather<Connection>.Listen(778)) {
                Console.WriteLine("Ready. Press any key to terminate.");
                Console.ReadKey(true);
            }
        }
    }
}
