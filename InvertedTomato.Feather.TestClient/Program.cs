using System;

// TODO: Tidy this example

namespace InvertedTomato.Feather.TestClient {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Connecting...");
            using (var client = Feather<Connection>.Connect("localhost", 777)) {
                Console.WriteLine("Ready. Press any key to send message.");

                while (true) {
                    Console.ReadKey(true);
                    client.SendMessage("Ben", "Hi there!");
                }
            }
        }
    }

    class Connection : ConnectionBase {
        public void SendMessage(string emailAddress, string password) {
            if (null == emailAddress) {
                throw new ArgumentNullException("emailAddress");
            }
            if (null == password) {
                throw new ArgumentNullException("password");
            }

            Send(new Payload(0x00).Append(emailAddress).Append(password));
        }

        protected override void OnMessageReceived(Payload payload) {
            switch (payload.Opcode) {
                case 0x00: // Chat message
                    var userName = payload.ReadString();
                    var message = payload.ReadString();

                    Console.WriteLine(userName + "> " + message);
                    break;
            }
        }
    }
}
