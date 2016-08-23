using System;

namespace InvertedTomato.Compression.Integers {
    public interface ISignedWriter : IDisposable {
        void Write(long value);

        // static byte[] Write (params long value);
        // static byte[] Write (IEnumerable<long> values);
    }
}
