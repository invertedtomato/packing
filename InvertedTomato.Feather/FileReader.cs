using System;
using System.IO;

namespace InvertedTomato.Feather {
    public class FileReader : IDisposable {
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
        private readonly FeatherFileOptions Options;

        /// <summary>
        /// Underlying file.
        /// </summary>
        private readonly FileStream UnderlyingFile;

        /// <summary>
        /// Synchronization lock.
        /// </summary>
        private readonly object Sync = new object();
        
        internal FileReader(string path, FeatherFileOptions options) {
            if (string.IsNullOrEmpty(path)) {
                throw new ArgumentException("Null or empty.", "path");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }

            // Store options
            Options = options;

            // Setup file stream
            UnderlyingFile = File.Open(path, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Read the next record. Returns null if no records remain.
        /// </summary>
        public PayloadReader Read() {
            byte[] rawPayload;

            lock (Sync) {
                if (IsDisposed) {
                    throw new ObjectDisposedException("Disposed.");
                }

                // Read payload
                ushort payloadLength;
                try {
                    payloadLength = UnderlyingFile.ReadUInt16();
                } catch (EndOfStreamException) {
                    return null;
                }
                rawPayload = UnderlyingFile.Read(payloadLength);
            }

            // Return payload
            return new PayloadReader(rawPayload);
        }

        /// <summary>
        /// Move cursor to start of the file.
        /// </summary>
        public void Rewind() {
            lock (Sync) {
                if (IsDisposed) {
                    throw new ObjectDisposedException("Disposed.");
                }

                // Rewind underlying file stream
                UnderlyingFile.Rewind();
            }
        }

        protected virtual void Dispose(bool disposing) {
            lock (Sync) {
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
        }
        public void Dispose() {
            Dispose(true);
        }
    }
}
