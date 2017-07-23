using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Compression.Integers {
    public interface IUnsignedCompressor {
        /// <summary>
        /// Compress a unsigned integer into a buffer.
        /// </summary>
        /// <param name="symbols">Value to compress</param>
        /// <param name="raw">Output buffer</param>
        void CompressUnsigned(ulong symbols, Buffer<byte> raw);

        /// <summary>
        /// Compress an array of unsigned integers into a buffer.
        /// </summary>
        /// <param name="symbols">Values to compress</param>
        /// <param name="raw">Output buffer</param>
        /// <returns>TRUE if all values fit in output.</returns>
        bool CompressUnsignedBuffer(Buffer<ulong> symbols, Buffer<byte> raw);
    }
}
