using InvertedTomato.Testable.Sockets;
using InvertedTomato.Testable.Streams;
using System;
using System.Diagnostics;
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
        public EndPoint RemoteEndPoint {
            get {
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
        private ConnectionOptions Options;

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
        public void Start(bool isServerConnection, ISocket clientSocket, ConnectionOptions options) {
            if (null == options) {
                throw new ArgumentNullException("options");
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
        /// Send single message to remote endpoint.
        /// </summary>    
        protected void Send(Payload payload) {
            Send(new Payload[] { payload }, null);
        }

        /// <summary>
        /// Send single message to remote endpoint and execute a callback when done.
        /// </summary>
        protected void Send(Payload payload, Action done) {
            Send(new Payload[] { payload }, done);
        }

        /// <summary>
        /// Send multiple messages to remote endpoint.
        /// </summary>    
        protected void Send(Payload[] payloads) {
            Send(payloads, null);
        }

        /// <summary>
        /// Send multiple messages to remote endpoint and execute a callback when done.
        /// </summary>
        protected void Send(Payload[] payloads, Action done) {
            if (null == payloads) {
                throw new ArgumentNullException("payload");
            }

            // Calculate total buffer length needed
            var bufferLength = 0;
            foreach(var payload in payloads) {
                // Check payload is not too long
                if (payload.Length > ushort.MaxValue) {
                    throw new ArgumentException("Payload too long for message. Total size of payload must be less than" + ushort.MaxValue + " bytes. " + payload.Length + " bytes given.", "payload");
                }

                // Sum lengths
                bufferLength += 2;
                bufferLength += payload.Length;
            }
            
            // Merge everthing to be sent into a buffer
            var buffer = new byte[bufferLength];
            var pos = 0;
            foreach (var payload in payloads) {
                Buffer.BlockCopy(BitConverter.GetBytes(payload.Length), 0, buffer, pos, 2); // Length
                buffer[pos+2] = payload.Opcode; // Opcode
                Buffer.BlockCopy(payload.Parameters, 0, buffer, pos+3, payload.Parameters.Length); // Parameters
                pos += 2 + payload.Length;
            }

            // Send
            RawSend(buffer, done);
        }

        private void RawSend(byte[] buffer, Action done) {
            // Increment outstanding counter
            Interlocked.Increment(ref OutstandingSends);

#if DEBUG
            // Show debug info
            Debug.WriteLine("TX: " + BitConverter.ToString(buffer));
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

        /// <summary>
        /// When a message arrives.
        /// </summary>
        /// <param name="payload"></param>
        protected abstract void OnMessageReceived(Payload payload);

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
                    Debug.WriteLine("RX:" + BitConverter.ToString(PayloadBuffer));
#endif
                    // Callback received message
                    OnMessageReceived(new Payload(PayloadBuffer));
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
            RawSend(new byte[] { 0, 0 }, null);
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
