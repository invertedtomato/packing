using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    /// Reader for VLQ signed numbers. Values are translated to unsigned values using ProtoBuffer's ZigZag algorithm.
    /// </summary>
    
    public class VLQSignedReader : ISignedReader {
        /// <summary>
        /// Read first value from a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Int64 ReadOneDefault(Byte[] input) {
            if (null == input) {
                throw new ArgumentNullException(nameof(input));
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new VLQSignedReader(stream)) {
                    return reader.Read();
                }
            }
        }

        /// <summary>
        /// If disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        /// Underlying unsigned reader.
        /// </summary>
        private readonly VLQUnsignedReader Underlying;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public VLQSignedReader(Stream input) {
            Underlying = new VLQUnsignedReader(input);
        }

        /// <summary>
        /// Instantiate with options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="packetSize">The number of bits to include in each packet.</param>
        public VLQSignedReader(Stream input, Int32 packetSize) {
            Underlying = new VLQUnsignedReader(input, packetSize);
        }
        
        /// <summary>
        /// Read the next value. 
        /// </summary>
        /// <returns></returns>
        public Int64 Read() {
            return ZigZag.Decode(Underlying.Read());
        }

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(Boolean disposing) {
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
        /// Dispose.
        /// </summary>
        public void Dispose() {
            Dispose(true);
        }
    }
}