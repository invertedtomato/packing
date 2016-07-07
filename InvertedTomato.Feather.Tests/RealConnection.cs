using System;
using System.Net;

namespace InvertedTomato.Feather.Tests {
    class RealConnection : ConnectionBase {
        public Action OnPingReceived;

        public void SendPing() {
            Send(0x01, new byte[] { 0x02 });
        }

        protected override void OnMessageReceived(byte opcode, byte[] payload) {
            if (opcode != 1) {
                throw new ProtocolViolationException("Unexpected opcode.");
            }
            if (payload.Length != 1) {
                throw new ProtocolViolationException("Unexpected length");
            }

            OnPingReceived.TryInvoke();
        }
    }
}
