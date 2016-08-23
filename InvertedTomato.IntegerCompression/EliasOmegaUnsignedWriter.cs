using InvertedTomato.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.IntegerCompression {
    /// <summary>
    /// Reader for Elias Omega universal coding for unsigned values.
    /// </summary>
    public class EliasOmegaUnsignedWriter : IUnsignedWriter {
        /// <summary>
        /// Write a given value.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static byte[] WriteOneDefault(ulong value) {
            using (var stream = new MemoryStream()) {
                using (var writer = new EliasOmegaUnsignedWriter(stream)) {
                    writer.Write(value);
                }
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Calculate the length of an encoded value in bits.
        /// </summary>
        /// <param name="allowZeros">(non-standard) Support zeros by automatically offsetting all values by one.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? CalculateBitLength(ulong value) {
            var result = 1; // Termination bit

            // Offset value to allow for 0s
            value++;

            // #2 If N=1, stop; encoding is complete.
            while (value > 1) {
                // Calculate the length of value
                var length = Bits.CountUsed(value);

                // #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
                result += length;

                // #4 Let N equal the number of bits just prepended, minus one.
                value = (ulong)length - 1;
            }

            return result;
        }

        /// <summary>
        /// If disposed.
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// Underlying stream to be writing encoded values to.
        /// </summary>
        private readonly BitWriter Output;

        /// <summary>
        /// Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public EliasOmegaUnsignedWriter(Stream output) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            Output = new BitWriter(output);
        }

        /// <summary>
        /// Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(ulong value) {
            if (IsDisposed) {
                throw new ObjectDisposedException("this");
            }

            // Offset min value
            value++;

            // Prepare buffer
            var groups = new Stack<KeyValuePair<ulong, byte>>();

            // #1 Place a "0" at the end of the code.
            groups.Push(new KeyValuePair<ulong, byte>(0, 1));

            // #2 If N=1, stop; encoding is complete.
            while (value > 1) {
                // Calculate the length of value
                var length = Bits.CountUsed(value);

                // #3 Prepend the binary representation of N to the beginning of the code (this will be at least two bits, the first bit of which is a 1)
                groups.Push(new KeyValuePair<ulong, byte>(value, length));

                // #4 Let N equal the number of bits just prepended, minus one.
                value = (ulong)length - 1;
            }

            // Write buffer
            foreach (var item in groups) {
                var bits = item.Value;
                var group = item.Key;

                Output.Write(group, bits);
            }
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

            if (disposing) {
                // Dispose managed state (managed objects)
                Output.DisposeIfNotNull();
            }
        }

        /// <summary>
        /// Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose() {
            Dispose(true);
        }
    }
}
