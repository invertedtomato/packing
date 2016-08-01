using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace InvertedTomato.Feather.Tests {
    [TestClass]
    public class ConnectionBaseTests {
        [TestMethod]
        public void Send() {
            using (var connection = new FakeConnection()) {
                Assert.AreEqual("01-00-01", BitConverter.ToString(connection.TestSend(0x01, new byte[] { })));
                Assert.AreEqual("09-00-02-01-02-03-04-05-06-07-08", BitConverter.ToString(connection.TestSend(0x02, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 })));
            }
        }
        [TestMethod]
        public void SendMany() {
            using (var connection = new FakeConnection()) {
                var payloads = new List<PayloadWriter>();
                payloads.Add(new PayloadWriter(0x83)
                    .Append(new Guid())
                    .Append("C1")
                    .Append("Floor1"));
                payloads.Add(new PayloadWriter(0x83)
                    .Append(new Guid())
                    .Append("C2")
                    .Append("Floorwe"));

                Assert.AreEqual("1D-00-83-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-02-00-43-31-06-00-46-6C-6F-6F-72-31-1E-00-83-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-00-02-00-43-32-07-00-46-6C-6F-6F-72-77-65", BitConverter.ToString(connection.TestSendMany(payloads.ToArray())));
            }
        }

        [TestMethod]
        public void Receive() {
            using (var connection = new FakeConnection()) {
                Assert.AreEqual("01", BitConverter.ToString(connection.TestReceive(new byte[] { 1, 0, 1 }))); // Opcode, no params
                Assert.AreEqual("01-02", BitConverter.ToString(connection.TestReceive(new byte[] { 2, 0, 1, 2 }))); // Opcode with params
                Assert.AreEqual("01-02-03-04-05-06-07-08", BitConverter.ToString(connection.TestReceive(new byte[] { 8, 0, 1, 2, 3, 4, 5, 6, 7, 8 })));
            }
        }

        [TestMethod]
        public void KeepAliveSend() {
            using (var connection = new FakeConnection(new FeatherTCPOptions() { ApplicationLayerKeepAlive = true, KeepAliveInterval = TimeSpan.FromMilliseconds(50) })) {
                // Allow time for keep-alive - timers are finicky with such small intervals
                Thread.Sleep(400);

                // Check they were sent
                var result = connection.Socket.Stream.ReadOutput();
                Assert.IsTrue(result.All(a => a == 0), "Incorrect data sent.");
                Assert.IsTrue(result.Length >= 4, "Not enough keep-alives received - " + result.Length + ".");
                Debug.Write(result.Length / 2 + " keep alives.");
            }
        }

        [TestMethod]
        public void EndToEnd() {
            var clientConnected = 0;
            var clientDisconnected = 0;
            var clientPings = 0;
            var serverPings = 0;
            var options = new FeatherTCPOptions() {
                NoDelay = true
            };

            // Create server
            using (var server = FeatherTCP<RealConnection>.Listen(1234, options)) {
                server.OnClientConnected += (connection) => {
                    clientConnected++;
                    connection.OnPingReceived += () => { serverPings++; };
                    connection.OnDisconnected += (reason) => { clientDisconnected++; };

                    // Send pings
                    connection.SendPing();
                    connection.SendPing();
                    connection.SendPing();
                };

                // Create client
                using (var client = FeatherTCP<RealConnection>.Connect("localhost", 1234, options)) {
                    client.OnPingReceived += () => { clientPings++; };

                    // Send pings
                    client.SendPing();
                    client.SendPing();

                    // Wait for sending to complete
                    Thread.Sleep(50);

                    // Dispose and check
                    client.Dispose();
                    Assert.IsTrue(client.IsDisposed);
                }

                // Wait for sending to complete
                Thread.Sleep(50);

                // Dispose and check
                server.Dispose();
                Assert.IsTrue(server.IsDisposed);
            }

            // Check all worked
            Assert.AreEqual(1, clientConnected);
            Assert.AreEqual(1, clientDisconnected);
            Assert.AreEqual(2, serverPings);
            Assert.AreEqual(3, clientPings);
        }

        // TODO:
        // Remote disconnect
        // Local disconnect
        // Connection interrupted
    }
}
