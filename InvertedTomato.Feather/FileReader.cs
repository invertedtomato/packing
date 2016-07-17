using System;
using System.IO;

namespace InvertedTomato.Feather {
    public class FileReader : IDisposable {
        /// <summary>
        /// Configuration options
        /// </summary>
        private readonly FileOptions Options;
        private readonly FileStream FileStream;

        /// <summary>
        /// If the file has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        private object Sync = new object();

        internal FileReader(string path, FileOptions options) {
            if (string.IsNullOrEmpty(path)) {
                throw new ArgumentException("Null or empty.", "path");
            }
            if (null == options) {
                throw new ArgumentNullException("options");
            }

            // Store options
            Options = options;

            // Setup file stream
            FileStream = File.Open(path, FileMode.Open, FileAccess.Read);
        }

        /// <summary>
        /// Read the next record. Returns null if no records remain.
        /// </summary>
        public Payload Read() {
            byte[] rawPayload;

            lock (Sync) {
                // Read payload
                ushort payloadLength;
                try {
                    payloadLength = FileStream.ReadUInt16();
                } catch (EndOfStreamException) {
                    return null;
                }
                rawPayload = FileStream.Read(payloadLength);
            }

            // Return payload
            return new Payload(rawPayload);
        }

        /// <summary>
        /// Move cursor to start of the file.
        /// </summary>
        public void Rewind() {
            // Rewind underlying file stream
            FileStream.Rewind();
        }

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                FileStream.DisposeIfNotNull();
            }

            // Set large fields to null
        }
        public void Dispose() {
            Dispose(true);
        }
    }
}
