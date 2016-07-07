using System;
using System.Net;

namespace InvertedTomato.Feather.Tests {
    class RealConnection : ConnectionBase {
        public Action OnPingReceived;

        public void SendPing() {
            Send(new Payload(0x01, new byte[] { 0x02 }));
        }

        protected override void OnMessageReceived(Payload payload) {
            if (payload.Opcode != 0x01) {
                throw new ProtocolViolationException("Unexpected opcode.");
            }
            if (payload.Parameters.Length != 1) {
                throw new ProtocolViolationException("Unexpected length");
            }

            OnPingReceived.TryInvoke();
        }
    }
}
