using System;
using System.Linq;
using InvertedTomato.Testable.Sockets;
using InvertedTomato.Testable.Streams;

namespace InvertedTomato.Feather.Tests {
    class FakeConnection : ConnectionBase {
        public readonly SocketFake Socket = new SocketFake();
        public byte[] LastPayload; 

        public FakeConnection() : this(new Options()) { }
        public FakeConnection(Options configuration) {
            if (null == configuration) {
                throw new ArgumentNullException("configuration");
            }
            
            Start(false, Socket, configuration);
        }

        public byte[] TestSend(byte[] send) {
            Send(send);

            return Socket.Stream.ReadOutput();
        }


        public byte[] TestReceive(byte[] wire) {
            Socket.Stream.QueueInput(wire);

            return LastPayload;
        }

        protected override void OnMessageReceived(byte[] payload) {
            LastPayload = payload;
        }
    }
}
