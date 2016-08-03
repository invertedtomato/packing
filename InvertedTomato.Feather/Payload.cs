using System;

namespace InvertedTomato.Feather {
    public interface IPayload : IDisposable {
        byte OpCode { get; }

        byte[] ToByteArray();
    }

    [Obsolete("Change to reference IPayload instead. Payload will be removed in a future release.")]
    public interface Payload : IPayload { }
}
