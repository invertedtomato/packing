using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers {
    public abstract class Codec : IUnsignedCompressor, IUnsignedDecompressor {
        /// <summary>
        /// Compress a unsigned integer into a buffer.
        /// </summary>
        /// <param name="symbol">Value to compress</param>
        /// <param name="raw">Output buffer</param>
        public abstract void CompressUnsigned(ulong symbol, Buffer<byte> raw);

        /// <summary>
        /// Compress an unsigned interger into a new buffer
        /// </summary>
        public virtual Buffer<byte> CompressUnsigned(ulong symbol) {
            var buffer = new Buffer<byte>(10);
            CompressUnsigned(symbol, buffer);
            return buffer;
        }

        /// <summary>
        /// Compress an unsigned interger into a new buffer
        /// </summary>
        public virtual Buffer<byte> CompressUnsigned(long symbol) {
#if DEBUG
            if (symbol < 0) {
                throw new ArgumentOutOfRangeException();
            }
#endif
            return CompressUnsigned((ulong)symbol);
        }

        /// <summary>
        /// Compress an array of unsigned integers into a buffer.
        /// </summary>
        /// <param name="symbols">Values to compress</param>
        /// <param name="raw">Output buffer</param>
        /// <returns>TRUE if all values fit in output.</returns>
        public virtual bool CompressUnsignedBuffer(Buffer<ulong> symbols, Buffer<byte> raw) {
#if DEBUG
            if (null == symbols) {
                throw new ArgumentNullException("symbols");
            }
            if (null == raw) {
                throw new ArgumentNullException("output");
            }
#endif
            // Dequeue from input until either the output is full, or the input has run dry
            while (raw.IsWritable && symbols.TryDequeue(out var symbol)) {
                CompressUnsigned(symbol, raw);
            }

            // Return whether the input has run dry
            return !symbols.IsReadable;
        }

        /// <summary>
        /// Compress an array of unsigned integers. This has no buffer control, and will be less memory efficent than CompressUnsignedBuffer.
        /// </summary>
        /// <param name="symbols">Values to compress</param>
        /// <returns>Compressed data</returns>
        public virtual byte[] CompressUnsignedArray(ulong[] symbols) {
#if DEBUG
            if (null == symbols) {
                throw new ArgumentNullException("symbols");
            }
#endif

            // Create buffers
            var input = new Buffer<ulong>(symbols);
            var output = new Buffer<byte>(symbols.Length * 10);

            // Compress
            CompressUnsignedBuffer(input, output);

            // Convert back to array
            return output.ToArray();
        }

        /// <summary>
        /// Compress an array of signed integers. This has no buffer control, and will be less memory efficent than CompressUnsignedBuffer.
        /// </summary>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public virtual byte[] CompressSignedArray(long[] symbols) {
#if DEBUG
            if (null == symbols) {
                throw new ArgumentNullException("symbols");
            }
#endif

            return CompressUnsignedArray(ZigZag.Encode(symbols));
        }

        /// <summary>
        /// Decompress a unsigned integer from a buffer.
        /// </summary>
        /// <param name="raw"></param>
        public abstract ulong DecompressUnsigned(Buffer<byte> raw);

        /// <summary>
        /// Decompress an array of unsigned integers from a buffer. If the input buffer runs empty, or the output buffer becomes full the process will halt.
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="symbols"></param>
        /// <returns>TRUE if all values fit in output.</returns>
        public virtual bool DecompressUnsignedBuffer(Buffer<byte> raw, Buffer<ulong> symbols) {
#if DEBUG
            if (null == raw) {
                throw new ArgumentNullException("input");
            }
            if (null == symbols) {
                throw new ArgumentNullException("symbols");
            }
#endif

            // While there is still input, and room to output symbols, keep decompressing
            while (raw.IsReadable && symbols.IsWritable) {
                symbols.Enqueue(DecompressUnsigned(raw));
            }

            // Return whether the input has run dry
            return !raw.IsReadable;
        }

        /// <summary>
        /// Decompress an array of unsigned integers. This has no buffer control, and will be less memory efficent than DecompressUnsignedBuffer.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public virtual ulong[] DecompressUnsignedArray(byte[] raw) {
#if DEBUG
            if (null == raw) {
                throw new ArgumentNullException("raw");
            }
#endif

            // Create buffers
            var input = new Buffer<byte>(raw);
            var output = new Buffer<ulong>(raw.Length * 5);

            // Compress
            DecompressUnsignedBuffer(input, output);

            // Convert back to array
            return output.ToArray();
        }

        /// <summary>
        /// Decompress an array of signed integers.  This has no buffer control, and will be less memory efficent than DecompressUnsignedBuffer.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public virtual long[] DecompressSignedArray(byte[] raw) {
            var symbols = DecompressUnsignedArray(raw);

            return ZigZag.Decode(symbols);
        }
    }
}
