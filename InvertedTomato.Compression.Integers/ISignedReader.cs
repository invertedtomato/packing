using System;

namespace InvertedTomato.Compression.Integers {
    public interface ISignedReader : IDisposable {
        long Read();

        // static long ReadOneDefault(byte[] input); 
    }
}
