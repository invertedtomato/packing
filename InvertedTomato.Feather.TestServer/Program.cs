using System;
using System.Collections.Concurrent;
using System.Net;
using System.Linq;

namespace InvertedTomato.Feather.TestServer {
    class Program {
        public static ConcurrentDictionary<EndPoint, Connection> Connections = new ConcurrentDictionary<EndPoint, Connection>();

        static void Main(string[] args) {
            // Start listening for connections
            using (var server = FeatherTCP<Connection>.Listen(777)) {
                // Watch for connections
                server.OnClientConnected += OnConnect;

                // Keep running until stopped
                Console.WriteLine("Chat server running. Press any key to terminate.");
                Console.ReadKey(true);
            }
        }

        static void OnConnect(Connection connection) {
            // Get remote end point
            var remoteEndPoint = connection.RemoteEndPoint;

            // Add to list of current connections
            Connections[remoteEndPoint] = connection;
            Console.WriteLine(remoteEndPoint.ToString() + " has connected.");

            // Setup to remove from connections on disconnect
            connection.OnDisconnected += (reason) => {
                Connections.TryRemove(remoteEndPoint, out connection);
                Console.WriteLine(remoteEndPoint.ToString() + " has disconnected.");
            };
        }
    }

    class Connection : ConnectionBase {
        protected override void OnMessageReceived(PayloadReader payload) {
            // Detect what type of message has arrived
            switch (payload.OpCode) {
                case 5:// Oh, it's a chat message
                    // Get parameters (in the same order they were sent)
                    var userName = payload.ReadString();
                    var message = payload.ReadString();

                    // Print it on the screen
                    Console.WriteLine(userName + "> " + message);

                    // Forward message to all OTHER clients
                    foreach (var connection in Program.Connections.Values.Where(a => a != this)) {
                        connection.Send(payload);
                    }
                    break;
                default:
                    // Report that an unknown opcode arrived
                    Console.WriteLine("Unknown message arrived with opcode " + payload.OpCode);
                    break;
            }
        }
    }
}
