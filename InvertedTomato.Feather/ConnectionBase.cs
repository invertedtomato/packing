using InvertedTomato.Testable.Sockets;
using InvertedTomato.Testable.Streams;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace InvertedTomato.Feather {
    /// <summary>
    /// Handles an incoming connection. Lasts the lifetime of the socket connection.
    /// </summary>
    public abstract partial class ConnectionBase : IDisposable {
        /// <summary>
        /// The remote endpoint.
        /// </summary>
        public EndPoint RemoteEndPoint
        {
            get
            {
                var clientSocket = ClientSocket;
                if (null == clientSocket) {
                    return null;
                }
                return clientSocket.RemoteEndPoint;
            }
        }

        /// <summary>
        /// The total amount of data transmitted (excluding headers).
        /// </summary>
        public long TotalTxBytes { get; private set; }

        /// <summary>
        /// The total amount of data received (excluding headers).
        /// </summary>
        public long TotalRxBytes { get; private set; }

        /// <summary>
        /// If the connection has been disposed (disconnected).
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Callback for when disconnection takes place.
        /// </summary>
        public Action<DisconnectionType> OnDisconnected;

        /// <summary>
        /// Number of messages that haven't made it to the TCP buffer yet. Will be used for back-pressure indication.
        /// </summary>
        private int OutstandingSends = 0;

        /// <summary>
        /// Configuration options
        /// </summary>
        private Options Options;

        /// <summary>
        /// Timer to send keep-alive messages to prevent disconnection.
        /// </summary>
        private System.Timers.Timer KeepAliveTimer;

        /// <summary>
        /// Time to disconnect if a message hasn't been received in a given amount of time.
        /// </summary>
        private System.Timers.Timer ReceiveTimeoutTimer;

        /// <summary>
        /// Client socket.
        /// </summary>
        private ISocket ClientSocket;

        /// <summary>
        /// Client stream.
        /// </summary>
        private IStream ClientStream;

        /// <summary>
        /// Receive buffers.
        /// </summary>
        private const int LengthTotalBytes = 2; // NOTE: Site for making change to length field size
        private int LengthReceivedBytes;
        private byte[] LengthBuffer;
        private int PayloadTotalBytes;
        private int PayloadRecievedBytes;
        private byte[] PayloadBuffer;

        /// <summary>
        /// Start the connection. Can only be called once.
        /// </summary>
        /// <param name="clientSocket"></param>
        /// <param name="options"></param>
        public void Start(bool isServerConnection, ISocket clientSocket, Options options) {
            if (null == options) {
                throw new ArgumentNullException("configuration");
            }
            if (null == clientSocket) {
                throw new ArgumentNullException("clientSocket");
            }
            if (null != ClientSocket) {
                throw new InvalidOperationException("Already started.");
            }

            // Store options
            Options = options;

            // Store and setup socket
            ClientSocket = clientSocket;
            ClientSocket.ReceiveBufferSize = Options.ReceiveBufferSize;
            ClientSocket.SendBufferSize = Options.SendBufferSize;
            ClientSocket.LingerState = Options.Linger;
            ClientSocket.NoDelay = Options.NoDelay;

            // Get stream
            if (!Options.IsSecure) {
                ClientStream = ClientSocket.GetStream();
            } else if (isServerConnection) {
                ClientStream = ClientSocket.GetSecureServerStream(Options.ServerCertificate);
            } else {
                ClientStream = ClientSocket.GetSecureClientStream(Options.ServerCommonName, ValidateServerCertificate);
            }

            // Start keep-alive timer (must be before receive start)
            KeepAliveTimer = new System.Timers.Timer(options.KeepAliveInterval);
            KeepAliveTimer.Elapsed += KeepAliveTimer_OnElapsed;
            KeepAliveTimer.Start();

            // Start keep-alive timer (must be before receive start)
            ReceiveTimeoutTimer = new System.Timers.Timer(options.ReceiveTimeout);
            ReceiveTimeoutTimer.Elapsed += ReceiveTimeoutTimer_OnElapsed; ;
            ReceiveTimeoutTimer.Start();

            // Seed receiving
            ReceiveLengthInit();
        }

        /// <summary>
        /// Send a binary payload to the remote endpoint.
        /// </summary>
        /// <param name="components"></param>      
        protected void Send(params byte[][] components) {
            if (null == components) {
                throw new ArgumentNullException("components");
            }

            using (var stream = new MemoryStream()) {
                // Merge all components into one payload  // TODO: Make more efficient by using Buffer.BlockCopy instead?
                foreach (var component in components) {
                    stream.Write(component);
                }

                // Send payload
                Send(stream.ToArray(), (Action)null);
            }
        }

        /// <summary>
        /// Send a binary payload to the remote endpoint.
        /// </summary>
        /// <param name="payload"></param>
        protected void Send(byte[] payload, Action done) {
            if (payload.Length > ushort.MaxValue) {
                throw new ArgumentException("Payload too long to write. Payload is " + payload.Length + " bytes. Must be between " + ushort.MinValue + " and " + ushort.MaxValue + " bytes.", "count");
            }

            byte[] buffer;
            using (var stream = new MemoryStream()) {

                stream.Write((ushort)payload.Length);
                stream.Write(payload);
                buffer = stream.ToArray();
            }
            // Increment outstanding counter
            Interlocked.Increment(ref OutstandingSends);
#if DEBUG
            Console.WriteLine("Sending: " + BitConverter.ToString(buffer));
#endif
            // Send
            try {
                ClientStream.BeginWrite(buffer, 0, buffer.Length, (ar) => {
                    try {
                        // Complete send
                        ClientStream.EndWrite(ar);
                        //Logging only for testing purposes.
                        Console.WriteLine(buffer);
                    } catch (ObjectDisposedException) {
                    } catch (IOException) {
                        // Report connection failure
                        DisconnectInner(DisconnectionType.ConnectionInterupted);
                        return;
                    } finally {
                        // Update total-TX counter
                        TotalTxBytes = unchecked(TotalTxBytes + buffer.Length);

                        // Decrement outstanding counter
                        Interlocked.Decrement(ref OutstandingSends);

                        // Callback success
                        done.TryInvoke();
                    }
                }, null);
            } catch (ObjectDisposedException) {
            } catch (IOException) {
                // Report connection failure
                DisconnectInner(DisconnectionType.ConnectionInterupted);
            }

            // Restart keep-alive timer
            KeepAliveTimer.Restart();
        }

        protected void SendMany(params byte[][] payloads) {
            SendMany(payloads, null);
        }

        /// <summary>
        /// Send a set of binary payload to the remote endpoint.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="done"></param>
        protected void SendMany(byte[][] payloads, Action done) {
            // Convert payloads to buffer  // TODO: Make more efficient by using Buffer.BlockCopy instead?
            byte[] buffer;
            using (var stream = new MemoryStream()) {
                foreach (var payload in payloads) {
                    stream.Write((ushort)payload.Length);
                    stream.Write(payload);
                }

                buffer = stream.ToArray();
                Send(buffer, done);
            }
        }

        /// <summary>
        /// When a message arrives.
        /// </summary>
        /// <param name="payload"></param>
        protected abstract void OnMessageReceived(byte[] payload);

        /// <summary>
        /// Disconnect from the remote endpoint and dispose.
        /// </summary>
        public void Disconnect() {
            DisconnectInner(DisconnectionType.LocalDisconnection);
        }

        private void ReceiveLengthInit() {
            // Prepare length buffer
            LengthReceivedBytes = 0;
            LengthBuffer = new byte[LengthTotalBytes];

            // Receive length
            ReceiveLengthBegin();
        }

        private void ReceiveLengthBegin() {
            // Check if we have all the data
            if (LengthReceivedBytes == LengthTotalBytes) {
                // Yes, get payload now
                ReceivePayloadInit();
                return;
            }

            try {
                // Request next chunk
                ClientStream.BeginRead(LengthBuffer, LengthReceivedBytes, LengthTotalBytes - LengthReceivedBytes, ReceiveLengthCallback, null);
            } catch (ObjectDisposedException) {
            } catch (IOException) {
                // Report connection failure
                DisconnectInner(DisconnectionType.ConnectionInterupted);
            }
        }

        private void ReceiveLengthCallback(IAsyncResult ar) {
            // Complete receive and get read length
            int bytesRead = 0;
            try {
                bytesRead = ClientStream.EndRead(ar);
            } catch (ObjectDisposedException) {
                return;
            } catch (IOException) {
                // Report connection failure
                DisconnectInner(DisconnectionType.ConnectionInterupted);
                return;
            }

            // Check if the remote has terminated the connection
            if (bytesRead == 0) {
                DisconnectInner(DisconnectionType.RemoteDisconnection);
                return;
            }

            // Update buffer counter
            LengthReceivedBytes += bytesRead;

            // Get next chunk
            ReceiveLengthBegin();
        }

        private void ReceivePayloadInit() {
            // Prepare payload
            PayloadTotalBytes = BitConverter.ToUInt16(LengthBuffer, 0);
            PayloadRecievedBytes = 0;
            PayloadBuffer = new byte[PayloadTotalBytes];

            // Update total-RX counter
            TotalRxBytes = unchecked(TotalTxBytes + PayloadTotalBytes);

            // Begin receive
            ReceivePayloadBegin();
        }

        private void ReceivePayloadBegin() {
            // If there is no more remaining
            if (PayloadRecievedBytes == PayloadTotalBytes) {
                // Kick the KeepAliveReceive timer
                ReceiveTimeoutTimer.Restart();

                // Yield payload
                if (PayloadTotalBytes > 0) { // Filter out keep-alive messages
#if DEBUG
                    Console.Write("Received:" + BitConverter.ToString(PayloadBuffer));
#endif
                    OnMessageReceived(PayloadBuffer);
                }

                // Receive next message
                ReceiveLengthInit();
                return;
            }

            try {
                // Request next chunk
                ClientStream.BeginRead(PayloadBuffer, PayloadRecievedBytes, PayloadTotalBytes - PayloadRecievedBytes, ReceivePayloadCallback, null);
            } catch (ObjectDisposedException) {
            } catch (IOException) {
                // Report connection failure
                DisconnectInner(DisconnectionType.ConnectionInterupted);
            }
        }

        private void ReceivePayloadCallback(IAsyncResult ar) {
            // Complete receive and get read length
            int bytesRead = 0;
            try {
                bytesRead = ClientStream.EndRead(ar);
            } catch (ObjectDisposedException) {
                return;
            } catch (IOException) {
                DisconnectInner(DisconnectionType.ConnectionInterupted);
                return;
            }

            // Handle connection reset
            if (bytesRead == 0) {
                DisconnectInner(DisconnectionType.RemoteDisconnection);
                return;
            }

            // Update counters
            PayloadRecievedBytes += bytesRead;

            // Get next chunk
            ReceivePayloadBegin();
        }

        /// <summary>
        /// Fires when message hasn't been received by the receive timeout.
        /// </summary>
        private void ReceiveTimeoutTimer_OnElapsed(object sender, System.Timers.ElapsedEventArgs e) {
            // Timed out - disconnect
            DisconnectInner(DisconnectionType.KeepAliveTimeout);
        }

        /// <summary>
        /// Fires when a message hasn't been sent in the keep-alive interval in order to prevent a receive-timeout on the remote end.
        /// </summary>
        private void KeepAliveTimer_OnElapsed(object sender, System.Timers.ElapsedEventArgs e) {
            // Send blank message - it will reset the timeout on the remote end, however not be delivered as an actual message
            Send(new byte[] { });
        }

        /// <summary>
        /// Handle internal disconnect requests.
        /// </summary>
        /// <param name="reason"></param>
        private void DisconnectInner(DisconnectionType reason) {
            if (IsDisposed) {
                return;
            }
            Dispose();

            OnDisconnected.TryInvoke(reason);
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Stop keep-alive sending
                KeepAliveTimer.StopIfNotNull();

                // Dispose managed state (managed objects)
                ClientStream.DisposeIfNotNull();

                var clientSocket = ClientSocket;
                if (null != clientSocket) {
                    try {
                        // Kill socket (being nice about it)
                        clientSocket.Close();
                    } catch { }

                    // Dispose socket
                    clientSocket.Dispose();
                }

                KeepAliveTimer.DisposeIfNotNull();
            }

            // Set large fields to null
            //ClientSocket = null;
            //ClientStream = null; // Do not set to null
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }


        /// <summary>
        /// Validate certificates given by servers, on the client end.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="policyErrors"></param>
        /// <returns></returns>
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors) {
            // If there are no errors, return success
            if (policyErrors == SslPolicyErrors.None) {
                return true;
            }

            // Do not allow this client to communicate with unauthenticated servers
            return false;
        }
    }

}
