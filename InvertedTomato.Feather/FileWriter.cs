using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvertedTomato.Extensions;

namespace InvertedTomato.Feather {
    public class FileWriter : IDisposable {
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
            FileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Write);
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
            var buffer = Feather.PayloadsToBuffer(payloads);

            lock (Sync) {
                if (IsDisposed) {
                    throw new ObjectDisposedException("Disposed.");
                }

                // Write raw payload to file
                FileStream.Write(buffer);
            }
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
