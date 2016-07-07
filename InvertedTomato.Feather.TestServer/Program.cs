using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvertedTomato;
using System.Security.Cryptography.X509Certificates;

namespace InvertedTomato.Feather.TestServer {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Starting servers (basic port=778, secure port=779)...");
            using (var server = Feather<Connection>.Listen(778)) {
                var options = new Options() {
                    IsSecure = true, // Make the connection secure
                    ServerCertificate = new X509Certificate2("choreographer_vlife_com_au.pfx", "r9FARQlT5") // Load the certificate the server must use to identify itself to clients
                };

                using (var secureServer = Feather<Connection>.Listen(779, options)) {
                    Console.WriteLine("Ready. Press any key to terminate.");
                    Console.ReadKey(true);
                }
            }
        }
    }
}
