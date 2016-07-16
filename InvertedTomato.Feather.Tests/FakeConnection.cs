using System;
using System.Linq;
using InvertedTomato.Testable.Sockets;
using InvertedTomato.Testable.Streams;

namespace InvertedTomato.Feather.Tests {
    class FakeConnection : ConnectionBase {
        public readonly SocketFake Socket = new SocketFake();
        public byte[] LastPayload;

        public FakeConnection() : this(new ConnectionOptions()) { }
        public FakeConnection(ConnectionOptions configuration) {
            if (null == configuration) {
                throw new ArgumentNullException("configuration");
            }

            Start(false, Socket, configuration);
        }

        public byte[] TestSend(byte opcode, byte[] payload) {
            Send(new Payload(opcode).AppendFixedLength(payload));

            return Socket.Stream.ReadOutput();
        }


        public byte[] TestReceive(byte[] wire) {
            Socket.Stream.QueueInput(wire);

            return LastPayload;
        }

        protected override void OnMessageReceived(Payload payload) {
            LastPayload = payload.ToByteArray();
        }
    }
}
