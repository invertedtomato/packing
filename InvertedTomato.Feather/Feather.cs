using InvertedTomato.Testable.Sockets;
using System;
using System.Net;
using System.Net.Sockets;

namespace InvertedTomato.Feather {

    [Obsolete("Use FeatherTCP instead.")]
    public static class Feather<TConnection> where TConnection : ConnectionBase, new() {
        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        [Obsolete("Use FeatherTCP instead.")]
        public static FeatherTCP<TConnection> Listen(int port) { return FeatherTCP<TConnection>.Listen(port); }

        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        [Obsolete("Use FeatherTCP instead.")]
        public static FeatherTCP<TConnection> Listen(int port, FeatherTCPOptions options) { return FeatherTCP<TConnection>.Listen(port, options); }

        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        [Obsolete("Use FeatherTCP instead.")]
        public static FeatherTCP<TConnection> Listen(EndPoint localEndPoint) { return FeatherTCP<TConnection>.Listen(localEndPoint); }

        /// <summary>
        /// Start a Feather server by listening for connections.
        /// </summary>
        /// <returns>Feather instance</returns>
        [Obsolete("Use FeatherTCP instead.")]
        public static FeatherTCP<TConnection> Listen(EndPoint endpoint, FeatherTCPOptions options) { return FeatherTCP<TConnection>.Listen(endpoint, options); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        [Obsolete("Use FeatherTCP instead.")]
        public static TConnection Connect(IPAddress serverAddress, int port) { return FeatherTCP<TConnection>.Connect(serverAddress, port); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        [Obsolete("Use FeatherTCP instead.")]
        public static TConnection Connect(IPAddress serverAddress, int port, FeatherTCPOptions options) { return FeatherTCP<TConnection>.Connect(serverAddress, port, options); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        [Obsolete("Use FeatherTCP instead.")]
        public static TConnection Connect(string serverName, int port) { return FeatherTCP<TConnection>.Connect(serverName, port); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        [Obsolete("Use FeatherTCP instead.")]
        public static TConnection Connect(string serverName, int port, FeatherTCPOptions options) { return FeatherTCP<TConnection>.Connect(serverName, port, options); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        [Obsolete("Use FeatherTCP instead.")]
        public static TConnection Connect(EndPoint endPoint) { return FeatherTCP<TConnection>.Connect(endPoint); }

        /// <summary>
        /// Connect to a Feather server.
        /// </summary>
        /// <returns>Server connection</returns>
        [Obsolete("Use FeatherTCP instead.")]
        public static TConnection Connect(EndPoint endPoint, FeatherTCPOptions options) { return FeatherTCP<TConnection>.Connect(endPoint, options); }
    }

    [Obsolete("Use FeatherFile instead.")]
    public sealed class Feather {
        /// <summary>
        /// Open Feather data file for reading.
        /// </summary>
        [Obsolete("Use FeatherFile instead.")]
        public static FileReader ReadFile(string path) { return FeatherFile.OpenRead(path); }

        /// <summary>
        /// Open Feather data file for reading.
        /// </summary>
        [Obsolete("Use FeatherFile instead.")]
        public static FileReader ReadFile(string path, FeatherFileOptions options) { return FeatherFile.OpenRead(path, options); }
        /// <summary>
        /// Open Feather data file for reading.
        /// </summary>
        [Obsolete("Use FeatherFile instead.")]
        public static FileWriter WriteFile(string path) { return FeatherFile.OpenWrite(path); }

        /// <summary>
        /// Open Feather data file for reading.
        /// </summary>
        [Obsolete("Use FeatherFile instead.")]
        public static FileWriter WriteFile(string path, FeatherFileOptions options) { return FeatherFile.OpenWrite(path, options); }
    }
}
