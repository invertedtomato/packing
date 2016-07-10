using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvertedTomato.Extensions;
using System.Threading;

namespace InvertedTomato.Feather {
    public class FileBase : IDisposable {
        /// <summary>
        /// Configuration options.
        /// </summary>
        private FileOptions Options;

        /// <summary>
        /// File stream.
        /// </summary>
        private FileStream FileStream;

        /// <summary>
        /// Number of appends to be performed before the next flush.
        /// </summary>
        private int UnflushedAppendsRemaining;

        /// <summary>
        /// If the file has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Sync lock.
        /// </summary>
        private object Sync = new object();

        /// <summary>
        /// Start the session. Can only be called once.
        /// </summary>
        public void Start(string path, FileOptions options) {
            if (null == options) {
                throw new ArgumentNullException("options");
            }

            lock (Sync) {
                // Fail if already started
                if (null != FileStream) {
                    throw new InvalidOperationException("Already started.");
                }

                // Store options
                Options = options;
                UnflushedAppendsRemaining = options.AppendFlushRate;

                // Setup file stream
                FileStream = File.Open(path, FileMode.OpenOrCreate);
            }
        }

        /// <summary>
        /// Read the next record. Returns null if no records remain.
        /// </summary>
        public Payload Read() {
            byte[] payload;

            lock (Sync) {
                // Stop if at end of file
                if (FileStream.Position == FileStream.Length) {
                    return null;
                }

                // Read payload
                var payloadLength = FileStream.ReadUInt16();
                payload = FileStream.Read(payloadLength);
            }

            // Return payload
            return new Payload(payload);
        }

        /// <summary>
        /// Move cursor to start of the file.
        /// </summary>
        public void Rewind() {
            lock (Sync) {
                FileStream.Position = 0;
            }
        }

        /// <summary>
        /// Clears buffer by causing all data to be writen to disk.
        /// </summary>
        public void Flush() { // TODO: Add unit tests
            lock (Sync) {
                FileStream.Flush();
            }
        }

        /// <summary>
        /// Append single payload to the end of the file.
        /// </summary>
        public void Append(Payload payload) {
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }

            lock (Sync) {
                // Appends payload to file
                Append(new Payload[] { payload });

                // Check if flush required
                if (Options.AppendFlushRate > 0 && --UnflushedAppendsRemaining <= 0) { // TODO: Add unit tests
                    UnflushedAppendsRemaining = Options.AppendFlushRate;

                    // Flush
                    Flush();
                }
            }
        }

        /// <summary>
        /// Append multiple payloads to the end of the file.
        /// </summary>
        public void Append(Payload[] payloads) {
            if (null == payloads) {
                throw new ArgumentNullException("payload");
            }

            // Convert to buffer
            var buffer = Feather.PayloadsToBuffer(payloads);

            lock (Sync) {
                FileStream.Position = FileStream.Length;
                FileStream.Write(buffer);
            }
        }


        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                FileStream.DisposeIfNotNull();
            }

            // Set large fields to null.
            FileStream = null;
        }
        public void Dispose() {
            Dispose(true);
        }
    }
}
