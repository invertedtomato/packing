using InvertedTomato.Testable.Streams;
using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace InvertedTomato.Testable.Sockets {
    public interface ISocket {
        bool NoDelay { get; set; }
        LingerOption LingerState { get; set; }
        int ReceiveBufferSize { get; set; }
        EndPoint RemoteEndPoint { get; }
        int SendBufferSize { get; set; }

        void Close();
        void Dispose();

        IStream GetStream();
        IStream GetSecureClientStream(string serverCommonName, RemoteCertificateValidationCallback serverCerficateValidationCallback);
        IStream GetSecureServerStream(X509Certificate serverCertificate);

        void SetKeepAlive(bool enabled);
        void SetKeepAlive(bool enabled, TimeSpan idle);
        void SetKeepAlive(bool enabled, TimeSpan idle, TimeSpan interval);
    }
}