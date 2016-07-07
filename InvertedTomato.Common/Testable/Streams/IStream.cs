using System;

namespace InvertedTomato.Testable.Streams {
    public interface IStream {
        IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state);
        int EndRead(IAsyncResult ar);
        IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state);
        void EndWrite(IAsyncResult ar);

        void Dispose();
    }
}
