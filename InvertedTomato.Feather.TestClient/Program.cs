using System;

namespace InvertedTomato.Feather.TestClient {
    class Program {
        static void Main(string[] args) {
            // Connect to server
            using (var client = FeatherTCP<Connection>.Connect("localhost", 777)) {
                // Get user's name
                Console.WriteLine("What's your name?");
                var userName = Console.ReadLine();

                // Go in a loop, sending any message the user types
                Console.WriteLine("Ready. Type your messages to send.");
                while (true) {
                    Console.Write(userName + "> ");
                    var message =  Console.ReadLine();
                    if (!string.IsNullOrEmpty(message)) {
                        client.SendMessage(userName, message);
                    }
                }
            }
        }
    }

    class Connection : ConnectionBase {
        public void SendMessage(string emailAddress, string message) {
            // Compose message payload ready for sending
            var payload = new PayloadWriter(5) // "5" is the opcode, identifying what type of message we're sending
                .Append(emailAddress)
                .Append(message);

            // Send it to the server
            Send(payload);
        }

        protected override void OnMessageReceived(PayloadReader payload) {
            // Detect what type of message has arrived
            switch (payload.OpCode) {
                case 5: // Oh, it's a chat message
                    // Get parameters (in the same order they were sent)
                    var userName = payload.ReadString();
                    var message = payload.ReadString();

                    // Print it on the screen
                    Console.WriteLine(userName + "> " + message);
                    break;
                default:
                    // Report that an unknown opcode arrived
                    Console.WriteLine("Unknown message arrived with opcode " + payload.OpCode);
                    break;
            }
        }
    }
}
