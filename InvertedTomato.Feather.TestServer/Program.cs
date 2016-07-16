using System;

// TODO: Tidy this example
namespace InvertedTomato.Feather.TestServer {
    class Program {
        public static Feather<Connection> Server;

        static void Main(string[] args) {
            Console.WriteLine("Starting server...");
            Server = Feather<Connection>.Listen(778);
            Console.WriteLine("Chat server running. Press any key to terminate.");
            Console.ReadKey(true);
            Server.Dispose();
        }
    }

    class Connection : ConnectionBase {
        protected override void OnMessageReceived(Payload payload) {
            switch (payload.Opcode) {
                case 0x00:
                    var userName = payload.ReadString();
                    var message = payload.ReadString();

                    Console.WriteLine(userName + "> " + message);
                    break;
            }
        }
    }
}
