using System;

namespace InvertedTomato.VariableLengthIntegers {
    public interface IUnsignedWriter : IDisposable {
        void Write(ulong value);

        // static byte[] Write (params ulong value);
        // static byte[] Write (IEnumerable<ulong> values);
    }
}
