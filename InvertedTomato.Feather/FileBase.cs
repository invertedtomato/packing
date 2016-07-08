using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InvertedTomato.Extensions;

namespace InvertedTomato.Feather {
    public class FileBase : IDisposable {
        /// <summary>
        /// Configuration options
        /// </summary>
        private FileOptions Options;
        private FileStream FileStream;

        /// <summary>
        /// If the file has been disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        private object Sync = new object();

        /// <summary>
        /// Start the session. Can only be called once.
        /// </summary>
        public void Start(string path, FileOptions options) {
            if (null == options) {
                throw new ArgumentNullException("options");
            }
            if (null != FileStream) {
                throw new InvalidOperationException("Already started.");
            }

            // Store options
            Options = options;

            // Setup file stream
            FileStream = File.Open(path, FileMode.OpenOrCreate);
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
            FileStream.Position = 0;
        }

        /// <summary>
        /// Append single payload to the end of the file.
        /// </summary>
        public void Append(Payload payload) {
            if (null == payload) {
                throw new ArgumentNullException("payload");
            }

            Append(new Payload[] { payload });
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

                // TODO: dispose managed state (managed objects).
                FileStream.DisposeIfNotNull();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
            // TODO: set large fields to null.


        }
        public void Dispose() {
            Dispose(true);
        }
    }
}
