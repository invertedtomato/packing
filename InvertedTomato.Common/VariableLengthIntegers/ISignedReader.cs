using System;

namespace InvertedTomato.VariableLengthIntegers {
    public interface ISignedReader : IDisposable {
        bool TryRead(out long value);
        long Read();

        // static IEnumerable<long> Read(/* options */, byte[] input); 
    }
}
