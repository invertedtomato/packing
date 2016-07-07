using System;
using System.IO;
using InvertedTomato;
using InvertedTomato.Feather;
using System.Collections.Generic;

namespace InvertedTomato.Feather.TestServer {
    class Connection : ConnectionBase {
        protected override void OnMessageReceived(Payload payload) {
            using (var stream = new MemoryStream(payload.Parameters)) {
                switch (payload.Opcode) {
                    case 0x00:
                        PrintMessage("GenerateAuthenticationKey", payload.Opcode, new Dictionary<string, string>() {
                            {"email_address", stream.ReadString() },
                            {"password", stream.ReadString() }
                        });
                        break;
                    default:
                        lock (ConsoleLock) {
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.Write(DateTime.UtcNow.ToString("HH:mm:ss") + " " + RemoteEndPoint.ToString() + " ");

                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("Unknown[0x" + BitConverter.ToString(new byte[] { payload.Opcode }) + "] ");

                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine("{");
                            Console.WriteLine("  " + BitConverter.ToString(stream.ReadRemaining()));
                            Console.WriteLine("}");
                        }
                        break;
                }
            }
        }

        private static object ConsoleLock = new object();
        private void PrintMessage(string name, byte opcode, Dictionary<string, string> properties) {
            lock (ConsoleLock) {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(DateTime.UtcNow.ToString("HH:mm:ss") + " " + RemoteEndPoint.ToString() + " ");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(name + "[0x" + BitConverter.ToString(new byte[] { opcode }) + "] ");

                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("{");
                foreach (var property in properties) {
                    Console.WriteLine("  " + property.Key + ": " + property.Value);
                }
                Console.WriteLine("}");
            }
        }
    }
}
