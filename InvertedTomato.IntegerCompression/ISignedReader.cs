using System;

namespace InvertedTomato.IntegerCompression {
    public interface ISignedReader : IDisposable {
        long Read();

        // static long ReadOneDefault(byte[] input); 
    }
}
