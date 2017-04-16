using System;
using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Compression.Integers {
    public interface IIntegerCodec {
        /// <summary>
        /// Compress a single input.
        /// </summary>
        /// <exception cref="OverflowException">Value could not be compressed as it exceeds the codec's supported range.</exception>
        void Compress(ulong input, Buffer<byte> output);

        /// <summary>
        /// Compress a given buffer.
        /// </summary>
        /// <exception cref="OverflowException">Value could not be compressed as it exceeds the codec's supported range.</exception>
        /// <returns>If all of the INPUT fit in the OUTPUT. False either indicates that a the output was not written at all or was partially written (resize output buffer and go again).</returns>
        bool Compress(Buffer<ulong> input, Buffer<byte> output);

        /// <summary>
        /// Decompress a single input.
        /// </summary>
        /// <exception cref="OverflowException">Value could not be compressed as it exceeds the codec's supported range.</exception>
        /// <exception cref="BufferOverflowException">Insufficent space in output buffer.</exception>
        /// <exception cref="FormatException">Fault in compressed data.</exception>
        /// <returns>The decompressed value.</returns>
        ulong Decompress(Buffer<byte> input);

        /// <summary>
        /// Decompress a given buffer. Stops when the output is full or when the input runs drop
        /// </summary>
        /// <exception cref="OverflowException">Value could not be compressed as it exceeds the codec's supported range.</exception>
        /// <exception cref="FormatException">Fault in compressed data.</exception>
        /// <returns>If all of the INPUT fit in the OUTPUT. False either indicates that a the output was not written at all or was partially written (resize output buffer and go again).</returns>
        bool Decompress(Buffer<byte> input, Buffer<ulong> output);
        

        /*
        // Compress
        Compressed = new Buffer<byte>(Input.Used * 2);
        while (!codec.Compress(Input, Compressed)) {
            Compressed = Compressed.Resize(Compressed.MaxCapacity * 2);
        }

        // Decompressed
        Output = new Buffer<ulong>(Compressed.Used);
        while (!codec.Decompress(Compressed, Output)) {
            Output = Output.Resize(Output.MaxCapacity * 2);
        }
    */
    }
}
