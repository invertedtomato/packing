using System;

namespace InvertedTomato.Feather {
    public interface IPayload : IDisposable {
        byte OpCode { get; }

        byte[] ToByteArray();
    }
}
