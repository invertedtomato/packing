using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using InvertedTomato.Testable.Streams;

namespace InvertedTomato.Testable.Sockets {
    public sealed class SocketFake : ISocket {
        public readonly StreamFake Stream = new StreamFake();

        public bool NoDelay { get; set; }

        public bool IsDisposed { get; private set; }

        public LingerOption LingerState { get { return null; } set { } }

        public int ReceiveBufferSize { get { return 0; } set { } }

        public EndPoint RemoteEndPoint { get { return null; } }

        public int SendBufferSize { get { return 0; } set { } }

        
        public void Close() {
            Dispose();
        }

        public void Dispose() {
            IsDisposed = true;
        }

        public IStream GetStream() {
            return Stream;
        }

        public IStream GetSecureClientStream(string serverCommonName, RemoteCertificateValidationCallback serverCerficateValidationCallback) {
            return Stream;
        }

        public IStream GetSecureServerStream(X509Certificate serverCertificate) {
            return Stream;
        }
    }
}
