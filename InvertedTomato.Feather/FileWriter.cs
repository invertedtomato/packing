using System;
using System.IO;

namespace InvertedTomato.Feather {
    public class FileWriter : IDisposable {
        /// <summary>
        /// If the file has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Length of the file, in bytes.
        /// </summary>
        public long Length { get { return UnderlyingFile.Length; } }

        /// <summary>
        /// Configuration options.
        /// </summary>
        private readonly FileOptions Options;

        /// <summary>
        /// Underlying file.
        /// </summary>
        private readonly FileStream UnderlyingFile;

        /// <summary>
        /// Synchronization lock.
        /// </summary>
        private readonly object Sync = new object();

        internal FileWriter(string path, FileOptions options) {
            if (string.IsNullOrEmpty(path)) {
                throw new ArgumentException("Null or empty.", "path");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }

            // Store options
            Options = options;

            // Setup file stream
            UnderlyingFile = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
        }

        /// <summary>
        /// Append single payload to the end of the file.
        /// </summary>
        public void Write(PayloadWriter payload) {
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }

            Write(new PayloadWriter[] { payload });
        }

        /// <summary>
        /// Append multiple payloads to the end of the file.
        /// </summary>
        public void Write(PayloadWriter[] payloads) {
            if (null == payloads) {
                throw new ArgumentNullException("payload");
            }

            // Convert to buffer
            var buffer = Core.PayloadsToBuffer(payloads);

            lock (Sync) {
                if (IsDisposed) {
                    throw new ObjectDisposedException("Disposed.");
                }

                // Write raw payload to file
                UnderlyingFile.Write(buffer);
            }
        }


        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                UnderlyingFile.DisposeIfNotNull();
            }

            // Set large fields to null
        }
        public void Dispose() {
            Dispose(true);
        }
    }
}
