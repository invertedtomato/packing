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
        /// Send a message to a Feather server.
        /// </summary>
        public void Send(IPAddress serverAddress, int port, Payload payload) { Send(new IPEndPoint(serverAddress, port), payload); }

        /// <summary>
        /// Send a message to a Feather server.
        /// </summary>
        public void Send(IPAddress serverAddress, int port, Payload payload, FeatherTCPOptions options) { Send(new IPEndPoint(serverAddress, port), payload, options); }

        /// <summary>
        /// Send a message to a Feather server.
        /// </summary>
        public void Send(string serverName, int port, Payload payload) { Send(new DnsEndPoint(serverName, port), payload); }

        /// <summary>
        /// Send a message to a Feather server.
        /// </summary>
        public void Send(string serverName, int port, Payload payload, FeatherTCPOptions options) { Send(new DnsEndPoint(serverName, port), payload, options); }

        /// <summary>
        /// Send a message to a Feather server.
        /// </summary>
        public void Send(EndPoint endPoint, Payload payload) { Send(endPoint, payload, new FeatherTCPOptions()); }

        /// <summary>
        /// Send a message to a Feather server.
        /// </summary>
        public void Send(EndPoint endPoint, Payload payload, FeatherTCPOptions options) {
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




        public static FeatherUDP Start() { return Start(new FeatherUDPOptions()); }

        public static FeatherUDP Start(FeatherUDPOptions options) {

            throw new NotImplementedException();
        }

        /// <summary>
        /// Start a Feather server by listening for messages.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherUDP StartAndBind(int bindPort) { return StartAndBind(new IPEndPoint(IPAddress.Any, bindPort)); }

        /// <summary>
        /// Start a Feather server by listening for messages.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherUDP StartAndBind(int bindPort, FeatherUDPOptions options) { return StartAndBind(new IPEndPoint(IPAddress.Any, bindPort), options); }

        /// <summary>
        /// Start a Feather server by listening for messages.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherUDP StartAndBind(EndPoint localEndPoint) { return StartAndBind(localEndPoint, new FeatherUDPOptions()); }

        /// <summary>
        /// Start a Feather server by listening for messages.
        /// </summary>
        /// <returns>Feather instance</returns>
        public static FeatherUDP StartAndBind(EndPoint localEndPoint, FeatherUDPOptions options) {
            if (null == localEndPoint) {
                throw new ArgumentNullException("endpoint");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }


            throw new NotImplementedException();

            return new FeatherUDP(localEndPoint, options);
        }


    }
}
