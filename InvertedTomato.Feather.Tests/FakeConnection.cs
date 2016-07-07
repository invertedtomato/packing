using System;
using System.Linq;
using InvertedTomato.Testable.Sockets;
using InvertedTomato.Testable.Streams;

namespace InvertedTomato.Feather.Tests {
    class FakeConnection : ConnectionBase {
        public readonly SocketFake Socket = new SocketFake();
        public byte LastOpcode;
        public byte[] LastPayload;

        public FakeConnection() : this(new Options()) { }
        public FakeConnection(Options configuration) {
            if (null == configuration) {
                throw new ArgumentNullException("configuration");
            }

            Start(false, Socket, configuration);
        }

        public byte[] TestSend(byte opcode, byte[] payload) {
            Send(opcode, payload);

            return Socket.Stream.ReadOutput();
        }


        public byte[] TestReceive(byte[] wire) {
            Socket.Stream.QueueInput(wire);

            var a = new byte[LastPayload.Length + 1];
            a[0] = LastOpcode;
            Buffer.BlockCopy(LastPayload, 0, a, 1, LastPayload.Length);

            return a;
        }

        protected override void OnMessageReceived(byte opcode, byte[] payload) {
            LastOpcode = opcode;
            LastPayload = payload;
        }
    }
}
