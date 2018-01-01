using System;

namespace InvertedTomato.Compression.Integers {
    public interface IUnsignedReader : IDisposable {
        UInt64 Read();

        // static ulong ReadOneDefault(); 
    }
}
