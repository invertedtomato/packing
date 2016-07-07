using System;
using System.Linq;
using InvertedTomato.Testable.Sockets;
using InvertedTomato.Testable.Streams;

namespace InvertedTomato.Feather.Tests {
    class FakeConnection : ConnectionBase {
        public readonly SocketFake Socket = new SocketFake();
        public byte LastOpcode;
        public byte[] LastParameters;

        public FakeConnection() : this(new ConnectionOptions()) { }
        public FakeConnection(ConnectionOptions configuration) {
            if (null == configuration) {
                throw new ArgumentNullException("configuration");
            }

            Start(false, Socket, configuration);
        }

        public byte[] TestSend(byte opcode, byte[] payload) {
            Send(new Payload(opcode, payload));

            return Socket.Stream.ReadOutput();
        }


        public byte[] TestReceive(byte[] wire) {
            Socket.Stream.QueueInput(wire);

            var a = new byte[LastParameters.Length + 1];
            a[0] = LastOpcode;
            Buffer.BlockCopy(LastParameters, 0, a, 1, LastParameters.Length);

            return a;
        }

        protected override void OnMessageReceived(Payload payload) {
            LastOpcode = payload.Opcode;
            LastParameters = payload.Parameters;
        }
    }
}
