using System;
using System.Diagnostics;
using System.IO;

namespace InvertedTomato.Feather.TestLoad {
    class Program {
        static void Main(string[] args) {
            using (var file = Feather.WriteFile("test.dat")) {
                for (var i = 1; i < 10000000; i++) {
                    file.Write(new Payload(0x00).Append(1).Append(2));
                }
            }

            using (var file = Feather.ReadFile("test.dat")) {
                Payload payload;
                while ((payload = file.Read()) != null) {
                    payload.ReadInt32();
                    payload.ReadInt32();
                }
            }
            
            File.Delete("test.dat");

            using (var server = Feather<TestConnection>.Listen(777)) {
                using (var client = Feather<TestConnection>.Connect("localhost", 777)) {
                    for (var i = 1; i < 100000; i++) {
                        client.TestSend();
                    }
                }
            }
        }
    }

    public class TestConnection : ConnectionBase {
        public void TestSend() {
            Send(new Payload(0x00).Append(1).Append(2));
        }
        protected override void OnMessageReceived(Payload payload) {
            payload.ReadInt32();
            payload.ReadInt32();
        }
    }
}
