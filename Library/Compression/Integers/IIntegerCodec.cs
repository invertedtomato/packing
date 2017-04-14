using InvertedTomato.IO.Buffers;
using System;

namespace InvertedTomato.Compression.Integers {
    interface IIntegerCodec {
        /// <summary>
        /// If a header is present.
        /// </summary>
        bool IncludeHeader { get; set; }

        /// <summary>
        /// Decompressed data. This overwritten populated by a Decompress(), or populated by you ready for a Compress(). 
        /// </summary>
        Buffer<ulong> DecompressedSet { get; set; }

        /// <summary>
        /// Compressed data. This is overwritten by a Compress(), or should be populated by you ready for a Decompress().
        /// </summary>
        Buffer<byte> CompressedSet { get; set; }

        /// <summary>
        /// Compress the contents of 'Decompressed' into 'Compressed'.
        /// </summary>
        /// <exception cref="InvalidOperationException">Indicates 'Decompressed' is not valid.</exception>
        /// <exception cref="OverflowException">Value could not be compressed as it exceeds the codex's supported range.</exception>
        void Compress();

        /// <summary>
        /// Attempt to decompress the contents of 'Compressed' into 'Decompressed'. This can be called even if you are not sure there is sufficent data in 'Compressed' to perform the decompression. If there isn't, it will return the number of additional bytes required.
        /// </summary>
        /// <exception cref="InvalidOperationException">Indicates 'Compressed' is not valid.</exception>
        /// <exception cref="OverflowException">Value to be decoded is larger than supported by codec's (typically 64-bits).</exception>
        /// <returns>0 if successful, otherwise the number of additional bytes required to attempt decompression again.</returns>
        int Decompress();
    }
}
