using System;
using System.Net;

namespace InvertedTomato.Feather.Tests {
    class RealConnection : ConnectionBase {
        public Action OnPingReceived;

        public void SendPing() {
            Send(new byte[] { 1 });
        }

        protected override void OnMessageReceived(byte[] payload) {
            if (payload.Length != 1) {
                throw new ProtocolViolationException("Unexpected length");
            }
            if(payload[0] != 1) {
                throw new ProtocolViolationException("Unexpected content.");
            }

            OnPingReceived.TryInvoke();
        }
    }
}
