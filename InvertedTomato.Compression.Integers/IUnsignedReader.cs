using System;

namespace InvertedTomato.Compression.Integers {
    public interface IUnsignedReader : IDisposable {
        ulong Read();

        // static ulong ReadOneDefault(); 
    }
}
