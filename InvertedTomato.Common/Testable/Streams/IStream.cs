using System;
using InvertedTomato.Interfaces;

namespace InvertedTomato.Testable.Streams {
    public interface IStream :IReadByte,IWriteByte{
        IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state);
        int EndRead(IAsyncResult ar);
        IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state);
        void EndWrite(IAsyncResult ar);

        void Dispose();
    }
}
