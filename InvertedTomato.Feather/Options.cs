using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace InvertedTomato.Feather {
    public sealed class Options {
        /// <summary>
        /// Use SSL to secure the connection.
        /// </summary>
        public bool IsSecure { get; set; } = false;

        /// <summary>
        /// Must be set if being used as a secure server, this certificate is used to prove identity to clients.
        /// </summary>
        public X509Certificate ServerCertificate { get; set; } = null;

        /// <summary>
        /// Must be set if being used as a secure client, this CN is used to verify the identity of the server.
        /// </summary>
        public string ServerCommonName { get; set; } = null;

        /// <summary>
        /// A keep-alive message will be sent after this amount of time if no other message has been sent.
        /// </summary>
        public int KeepAliveInterval { get; set; } = 15000; // ms;

        /// <summary>
        /// Disconnect if a message hasn't been received in this length of time
        /// </summary>
        public int ReceiveTimeout { get; set; } = 30000; // ms

        /// <summary>
        /// Size of receive buffer before blocking occurs.
        /// </summary>
        public int ReceiveBufferSize { get; set; } = 8 * 1024; // bytes

        /// <summary>
		/// Size of send buffer before blocking occurs.
		/// </summary>
		public int SendBufferSize { get; set; } = 8 * 1024; // bytes

        /// <summary>
        /// How lingering is handled.
        /// </summary>
        public LingerOption Linger { get; set; } = new LingerOption(true, 250);

        /// <summary>
        /// Maximum number of pending connections for a listener.
        /// </summary>
        public int MaxListenBacklog { get; set; } = 16;

        /// <summary>
        /// Disable the Nagle algorithm to send data immediately, rather than delaying in the hopes of packing more data into packets.
        /// </summary>
        public bool NoDelay { get; set; } = false;
    }
}
