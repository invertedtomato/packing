using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Compression.Integers {
    public interface IUIntDecompressor {
        ulong DecompressUInt(Buffer<byte> input);

        ulong[] DecompressUIntArray(Buffer<byte> input);
    }
}
