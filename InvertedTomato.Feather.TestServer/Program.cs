using System;

// TODO: Tidy this example
namespace InvertedTomato.Feather.TestServer {
    class Program {
        public static FeatherTCP<Connection> Server;

        static void Main(string[] args) {
            Console.WriteLine("Starting server...");
            Server = FeatherTCP<Connection>.Listen(777);
            Console.WriteLine("Chat server running. Press any key to terminate.");
            Console.ReadKey(true);
            Server.Dispose();
        }
    }

    class Connection : ConnectionBase {
        protected override void OnMessageReceived(PayloadReader payload) {
            switch (payload.OpCode) {
                case 0x00:
                    var userName = payload.ReadString();
                    var message = payload.ReadString();

                    Console.WriteLine(userName + "> " + message);
                    break;
            }
        }
    }
}
