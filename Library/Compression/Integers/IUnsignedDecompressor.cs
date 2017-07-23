using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Compression.Integers {
    public interface IUnsignedDecompressor {
        /// <summary>
        /// Decompress a unsigned integer from a buffer.
        /// </summary>
        /// <param name="input"></param>
        ulong DecompressUnsigned(Buffer<byte> input);

        /// <summary>
        /// Decompress an array of unsigned integers from a buffer. If the input buffer runs empty, or the output buffer becomes full the process will halt.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="symbols"></param>
        /// <returns>TRUE if all values fit in output.</returns>
        bool DecompressUnsignedBuffer(Buffer<byte> input, Buffer<ulong> symbols);
    }
}
