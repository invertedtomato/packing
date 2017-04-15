using System;
using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Compression.Integers {
    public interface IIntegerCodec {

        /// <summary>
        /// Compress a single input.
        /// </summary>
        /// <exception cref="OverflowException">Value could not be compressed as it exceeds the codec's supported range.</exception>
        /// <exception cref="InvalidOperationException">There was insufficent output buffer space a single value.</exception>
        void CompressOne(ulong input, Buffer<byte> output);
        
        /// <summary>
        /// Compress all of the provided input.
        /// </summary>
        /// <exception cref="OverflowException">Value could not be compressed as it exceeds the codec's supported range.</exception>
        /// <returns>The number of inputs that was processed. If this isn't all of the inputs, then the output buffer is full.</returns>
        int CompressMany(Buffer<ulong> input, Buffer<byte> output);
        
        /// <summary>
        /// Decompress a single input.
        /// </summary>
        /// <exception cref="OverflowException">Value to be decoded is larger than supported by codec's (typically 64-bits).</exception>
        /// <exception cref="InvalidOperationException">There was insufficent input data for a single value.</exception>
        /// <returns>The decompressed value.</returns>
        ulong DecompressOne(Buffer<byte> input);

        /// <summary>
        /// Decompress COUNT of the provided input.
        /// </summary>
        /// <exception cref="OverflowException">Value to be decoded is larger than supported by codec's (typically 64-bits).</exception>
        /// <returns>The number of outputs that were processed. If this isn't all of the values, then the output buffer is full.</returns>
        int DecompressMany(Buffer<byte> input, Buffer<ulong> output);
    }
}
