using System;

namespace InvertedTomato.IntegerCompression {
    public interface IUnsignedReader : IDisposable {
        bool TryRead(out ulong value);
        ulong Read();

        // static IEnumerable<ulong> Read(/* options */, byte[] input); 
    }
}
