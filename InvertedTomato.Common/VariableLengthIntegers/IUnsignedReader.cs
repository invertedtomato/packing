using System;

namespace InvertedTomato.VariableLengthIntegers {
    public interface IUnsignedReader : IDisposable {
        bool TryRead(out ulong value);
        ulong Read();

        // static IEnumerable<ulong> Read(/* options */, byte[] input); 
    }
}
