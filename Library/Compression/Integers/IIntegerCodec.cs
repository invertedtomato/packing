using System;
using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Compression.Integers {
    public interface IIntegerCodec {
        /// <summary>
        /// Compress a single input.
        /// </summary>
        /// <exception cref="OverflowException">Value could not be compressed as it exceeds the codec's supported range.</exception>
        /// <exception cref="BufferOverflowException">Insufficent space in output buffer.</exception>
        void Compress(ulong input, Buffer<byte> output);

        /// <summary>
        /// Compress a given buffer.
        /// </summary>
        /// <exception cref="OverflowException">Value could not be compressed as it exceeds the codec's supported range.</exception>
        /// <returns>If all of the input was processed.</returns>
        bool Compress(Buffer<ulong> input, Buffer<byte> output);

        /// <summary>
        /// Decompress a single input.
        /// </summary>
        /// <exception cref="OverflowException">Value could not be compressed as it exceeds the codec's supported range.</exception>
        /// <exception cref="BufferOverflowException">Insufficent space in output buffer.</exception>
        /// <returns>The decompressed value.</returns>
        ulong Decompress(Buffer<byte> input);

        /// <summary>
        /// Decompress a given buffer. Stops when the output is full or when the input runs drop
        /// </summary>
        /// <exception cref="OverflowException">Value could not be compressed as it exceeds the codec's supported range.</exception>
        /// <returns>If all of the output was filled..</returns>
        bool Decompress(Buffer<byte> input, Buffer<ulong> output);
    }
}
