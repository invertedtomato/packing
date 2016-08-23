using System;

namespace InvertedTomato.IntegerCompression {
    public interface IUnsignedReader : IDisposable {
        ulong Read();

        // static ulong ReadOneDefault(); 
    }
}
