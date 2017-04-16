using System;
using System.IO;

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    /// Writer for Elias Gamma universal coding adapted for signed values.
    /// </summary>
    
    public class EliasGammaSignedWriter : ISignedWriter {
        /// <summary>
        /// Write a given value.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteOneDefault(long value) {
            using (var stream = new MemoryStream()) {
                using (var writer = new EliasGammaSignedWriter(stream)) {
                    writer.Write(value);
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// If disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// The underlying unsigned writer.
        /// </summary>
        private readonly EliasGammaUnsignedWriter Underlying;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public EliasGammaSignedWriter(Stream output) {
            Underlying = new EliasGammaUnsignedWriter(output);
        }

        /// <summary>
        /// Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(long value) {
            Underlying.Write(ZigZag.Encode(value));
        }

        /// <summary>
        /// Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            Underlying.Dispose();

            if (disposing) {
                // Dispose managed state (managed objects)
                Underlying?.Dispose();
            }
        }

        /// <summary>
        /// Flush any unwritten bits and dispose.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
    }
}
