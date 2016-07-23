using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato.Feather {
    public sealed class FeatherUDP {
        /// <summary>
        /// Start a Feather server by listening for messages.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherUDP Bind(int port) { return Bind(new IPEndPoint(IPAddress.Any, port)); }

        /// <summary>
        /// Start a Feather server by listening for messages.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherUDP Bind(int port, FeatherUDPOptions options) { return Bind(new IPEndPoint(IPAddress.Any, port), options); }

        /// <summary>
        /// Start a Feather server by listening for messages.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherUDP Bind(EndPoint localEndPoint) { return Bind(localEndPoint, new FeatherUDPOptions()); }

        /// <summary>
        /// Start a Feather server by listening for messages.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherUDP Bind(EndPoint localEndPoint, FeatherUDPOptions options) {
            if (null == localEndPoint) {
                throw new ArgumentNullException("endpoint");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }

            return new FeatherUDP(localEndPoint, options);
        }

        /// <summary>
        /// Send a message to a Feather server.
        /// </summary>
        public static void Send(IPAddress serverAddress, int port, Payload payload) { Send(new IPEndPoint(serverAddress, port), payload); }

        /// <summary>
        /// Send a message to a Feather server.
        /// </summary>
        public static void Send(IPAddress serverAddress, int port, Payload payload, FeatherTCPOptions options) { Send(new IPEndPoint(serverAddress, port), payload, options); }

        /// <summary>
        /// Send a message to a Feather server.
        /// </summary>
        public static void Send(string serverName, int port, Payload payload) { Send(new DnsEndPoint(serverName, port), payload); }

        /// <summary>
        /// Send a message to a Feather server.
        /// </summary>
        public static void Send(string serverName, int port, Payload payload, FeatherTCPOptions options) { Send(new DnsEndPoint(serverName, port), payload, options); }

        /// <summary>
        /// Send a message to a Feather server.
        /// </summary>
        public static void Send(EndPoint endPoint, Payload payload) { Send(endPoint, payload, new FeatherTCPOptions()); }

        /// <summary>
        /// Send a message to a Feather server.
        /// </summary>
        public static void Send(EndPoint endPoint, Payload payload, FeatherTCPOptions options) {
            if (null == endPoint) {
                throw new ArgumentNullException("endPoint");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }

            // Create payload
            var buffer = payload.ToByteArray(); // DO NOT USE Core.PayloadsToBuffer - not required for UDP

            // Open socket
            using (var clientSocket = new Socket(SocketType.Stream, ProtocolType.Udp)) {
                clientSocket.SendTo(buffer, endPoint);
            }
        }
    }
}
