using InvertedTomato.IO.Buffers;

namespace InvertedTomato.Compression.Integers {
    public interface IUIntCompressor {
        void CompressUInt(ulong symbol, Buffer<byte> output);

        void CompressUIntArray(ulong[] symbol, Buffer<byte> output);
    }
}
