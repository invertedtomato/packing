using System;
using System.Net;

namespace InvertedTomato.Feather.Tests {
    class RealConnection : ConnectionBase {
        public Action OnPingReceived;

        public void SendPing() {
            Send(new PayloadWriter(0x01).Append(0x02));
        }

        protected override void OnMessageReceived(PayloadReader payload) {
            if (payload.Opcode != 0x01) {
                throw new ProtocolViolationException("Unexpected opcode.");
            }
            if (payload.Length != 2) {
                throw new ProtocolViolationException("Unexpected length");
            }

            OnPingReceived.TryInvoke();
        }
    }
}
