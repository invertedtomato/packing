using InvertedTomato.Testable.Streams;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace InvertedTomato.Testable.Sockets {
    public sealed class SocketReal : ISocket {
        private readonly Socket Socket;

        public SocketReal(Socket socket) {
            if (null == socket) {
                throw new ArgumentNullException("socket");
            }

            Socket = socket;
        }

        public bool NoDelay { get { return Socket.NoDelay; } set { Socket.NoDelay = value; } }
        public LingerOption LingerState { get { return Socket.LingerState; } set { Socket.LingerState = value; } }
        public int ReceiveBufferSize { get { return Socket.ReceiveBufferSize; } set { Socket.ReceiveBufferSize = value; } }
        public EndPoint RemoteEndPoint { get { return Socket.RemoteEndPoint; } }
        public int SendBufferSize { get { return Socket.SendBufferSize; } set { Socket.SendBufferSize = value; } }
        public void Close() { Socket.Close(); }
        public void Dispose() { Socket.Dispose(); }

        public IStream GetStream() { return new StreamReal(new NetworkStream(Socket, true)); }

        public void SetKeepAlive(bool enabled) { Socket.SetKeepAlive(enabled); }
        public void SetKeepAlive(bool enabled, TimeSpan idle) { Socket.SetKeepAlive(enabled, idle); }
        public void SetKeepAlive(bool enabled, TimeSpan idle, TimeSpan interval) { Socket.SetKeepAlive(enabled, idle, interval); }

        public IStream GetSecureClientStream(string serverCommonName, RemoteCertificateValidationCallback serverCerficateValidationCallback) {
            var secureClientStream = new SslStream(new NetworkStream(Socket, true), false, serverCerficateValidationCallback, null, EncryptionPolicy.RequireEncryption);
            secureClientStream.AuthenticateAsClient(serverCommonName);
            return new StreamReal(secureClientStream);
        }

        public IStream GetSecureServerStream(X509Certificate serverCertificate) {
            var secureClientStream = new SslStream(new NetworkStream(Socket, true), false);
            secureClientStream.AuthenticateAsServer(serverCertificate, false, SslProtocols.Tls, true);
            return new StreamReal(secureClientStream);
        }
    }
}
