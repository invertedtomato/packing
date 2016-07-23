using InvertedTomato.Testable.Sockets;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace InvertedTomato.Feather {
    public sealed class FeatherTCP<TConnection> : IDisposable where TConnection : ConnectionBase, new() {
        /// <summary>
        /// When a client connects.
        /// </summary>
        public Action<TConnection> OnClientConnected;

        /// <summary>
        /// Has the server been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// User provided options.
        /// </summary>
        private readonly FeatherTCPOptions Options;

        /// <summary>
        /// Socket the server is listening on.
        /// </summary>
        private readonly Socket ListenerSocket;

        internal FeatherTCP(EndPoint endPoint, FeatherTCPOptions options) {
            // Store configuration
            Options = options;

            try {
                // Open socket
                ListenerSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                ListenerSocket.Bind(endPoint);
                ListenerSocket.Listen(Options.MaxListenBacklog);

                // Seed accepting
                AcceptBegin();
            } catch (ObjectDisposedException) { } // This occurs if the server is disposed during instantiation
        }

        private void AcceptBegin() {
            // Wait for, and accept next connection
            ListenerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void AcceptCallback(IAsyncResult ar) {
            try {
                // Get client socket
                var clientSocket = ListenerSocket.EndAccept(ar);

                // Create connection
                var connection = new TConnection();
                connection.Start(true, new SocketReal(clientSocket), Options);

                // Raise event
                OnClientConnected.TryInvoke(connection);

                // Resume accepting sockets
                AcceptBegin();
            } catch (ObjectDisposedException) { } // This occurs naturally during dispose
        }

        /// <summary>
        /// Send a message to all clients. The last message for each topic will be stored and delivered to newly connecting clients. 
        /// NOTE: If clients are unable to receive messages at the rate they are being sent, messages will be dropped intelligently.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="payload"></param>
        public void Broadcast(object topic, Payload payload) {
            throw new NotImplementedException("Broadcast functionality not yet implemented by Ben Thompson");
        }

        public void Dispose() { Dispose(true); }
        void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                ListenerSocket.Dispose();
            }
        }


        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherTCP<TConnection> Listen(int port) { return Listen(new IPEndPoint(IPAddress.Any, port)); }

        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherTCP<TConnection> Listen(int port, FeatherTCPOptions options) { return Listen(new IPEndPoint(IPAddress.Any, port), options); }

        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherTCP<TConnection> Listen(EndPoint localEndPoint) { return Listen(localEndPoint, new FeatherTCPOptions()); }

        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherTCP<TConnection> Listen(EndPoint localEndPoint, FeatherTCPOptions options) {
            if (null == localEndPoint) {
                throw new ArgumentNullException("endpoint");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }

            return new FeatherTCP<TConnection>(localEndPoint, options);
        }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public static TConnection Connect(IPAddress serverAddress, int port) { return Connect(new IPEndPoint(serverAddress, port)); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public static TConnection Connect(IPAddress serverAddress, int port, FeatherTCPOptions options) { return Connect(new IPEndPoint(serverAddress, port), options); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public static TConnection Connect(string serverName, int port) { return Connect(new DnsEndPoint(serverName, port)); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public static TConnection Connect(string serverName, int port, FeatherTCPOptions options) { return Connect(new DnsEndPoint(serverName, port), options); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public static TConnection Connect(EndPoint endPoint) { return Connect(endPoint, new FeatherTCPOptions()); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        public static TConnection Connect(EndPoint endPoint, FeatherTCPOptions options) {
            if (null == endPoint) {
                throw new ArgumentNullException("endPoint");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }

            // Open socket
            var clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(endPoint);

            // Create connection
            var connection = new TConnection();
            connection.Start(false, new SocketReal(clientSocket), options);

            return connection;
        }
    }
}
