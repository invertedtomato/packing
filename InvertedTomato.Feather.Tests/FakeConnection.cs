using System;
using InvertedTomato.Testable.Sockets;

namespace InvertedTomato.Feather.Tests {
	class FakeConnection : ConnectionBase {
		public readonly SocketFake Socket = new SocketFake();
		public byte[] LastPayload;

		public FakeConnection() : this(new FeatherTCPOptions()) { }
		public FakeConnection(FeatherTCPOptions configuration) {
			if (null == configuration) {
				throw new ArgumentNullException("configuration");
			}

			Start(false, Socket, configuration);
		}

		public byte[] TestSend(byte opcode, byte[] payload) {
			Send(new PayloadWriter(opcode).AppendFixedLength(payload));

			return Socket.Stream.ReadOutput();
		}
		public byte[] TestSendMany(Payload[] payloads) {
			Send(payloads);

			return Socket.Stream.ReadOutput();
		}

		public byte[] TestReceive(byte[] wire) {
			Socket.Stream.QueueInput(wire);

			return LastPayload;
		}

		protected override void OnMessageReceived(PayloadReader payload) {
			LastPayload = payload.ToByteArray();
		}
	}
}
