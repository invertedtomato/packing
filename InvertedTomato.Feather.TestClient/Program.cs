using System;

namespace InvertedTomato.Feather.TestClient {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Connecting...");
            var options = new ConnectionOptions() {
                IsSecure = true, // Make the connection secure
                ServerCommonName = "choreographer.vlife.com.au" // Set the name of the certificate that the server must have (this stops someone impersonating the server)
            };

            using (var secureClient = Feather<Connection>.Connect("localhost", 779, options)) {
                Console.WriteLine("Ready. Press any key to send message.");

                while (true) {
                    Console.ReadKey(true);
                    secureClient.GenerateAuthenticationKey("ben@invertedtomato.com", "lotsofcheesecake");
                    Console.WriteLine("GenerateAuthenticationKey sent.");
                }
            }
        }
    }
}
