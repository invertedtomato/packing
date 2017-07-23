using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers {
    public abstract class Codec : IUnsignedCompressor, IUnsignedDecompressor {
        public abstract void CompressUnsigned(ulong symbol, Buffer<byte> output);

        public virtual bool CompressUnsignedBuffer(Buffer<ulong> symbols, Buffer<byte> output) {
#if DEBUG
            if (null == symbols) {
                throw new ArgumentNullException("symbols");
            }
            if (null == output) {
                throw new ArgumentNullException("output");
            }
#endif
            // Dequeue from input until either the output is full, or the input has run dry
            while (output.IsWritable&& symbols.TryDequeue(out var symbol)) {
                CompressUnsigned(symbol, output);
            }

            // Return whether the input has run dry
            return !symbols.IsReadable;
        }


        public abstract ulong DecompressUnsigned(Buffer<byte> input);

        public virtual bool DecompressUnsignedBuffer(Buffer<byte> input, Buffer<ulong> symbols) {
#if DEBUG
            if (null == input) {
                throw new ArgumentNullException("input");
            }
            if (null == symbols) {
                throw new ArgumentNullException("symbols");
            }
#endif

            // While there is still input, and room to output symbols, keep decompressing
            while (input.IsReadable && symbols.IsWritable) {
                symbols.Enqueue(DecompressUnsigned(input));
            }

            // Return whether the input has run dry
            return !input.IsReadable;
        }
    }
}
