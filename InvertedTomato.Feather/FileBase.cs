using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Payload Read() {
            throw new NotImplementedException();
        }

        public void Rewind() {
            throw new NotImplementedException();
        }

        public void Append(Payload payload) {
            throw new NotImplementedException();
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
