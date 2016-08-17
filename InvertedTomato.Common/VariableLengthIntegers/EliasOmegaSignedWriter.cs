using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
    /// <summary>
    /// Writer for Elias Omega universal coding adapted for signed values. Values are translated to unsigned values using ProtoBuffers ZigZag algorithm.
    /// 
    /// Per Elias standard, zeros are not normally supported. They can however be supported by an offset technique - pass TRUE in the constructor to allow this functionality.
    /// </summary>
    public class EliasOmegaSignedWriter : ISignedWriter {
        /// <summary>
        /// Write all given values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(IEnumerable<long> values) { return WriteAll(false, values); }

        /// <summary>
        /// Write all given values with options.
        /// </summary>
        /// <param name="allowZeros">(non-standard) Support zeros by automatically offsetting all values by one.</param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteAll(bool allowZeros, IEnumerable<long> values) {
            using (var stream = new MemoryStream()) {
                using (var writer = new EliasOmegaSignedWriter(stream, allowZeros)) {
                    foreach (var value in values) {
                        writer.Write(value);
                    }
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
        private readonly EliasOmegaUnsignedWriter Underlying;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public EliasOmegaSignedWriter(Stream output) {
            Underlying = new EliasOmegaUnsignedWriter(output);
        }

        /// <summary>
        /// Instantiate with options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="allowZero">(non-standard) Support zeros by automatically offsetting all values by one.</param>
        public EliasOmegaSignedWriter(Stream input, bool allowZero) {
            Underlying = new EliasOmegaUnsignedWriter(input, allowZero);
        }

        /// <summary>
        /// Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(long value) {
            var innerValue = ZigZag.Encode(value);
            Underlying.Write(innerValue);
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
                // Dispose managed state (managed objects).
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
