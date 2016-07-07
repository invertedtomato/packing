using System;
using System.IO;

namespace InvertedTomato.Testable.Streams {
    public sealed class StreamReal : IStream {
        private readonly Stream Stream;

        public StreamReal(Stream stream) {
            if (null == stream) {
                throw new ArgumentNullException("stream");
            }

            Stream = stream;
        }

        public IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
            return Stream.BeginRead(buffer, offset, count, callback, state);
        }

        public int EndRead(IAsyncResult ar) {
            return Stream.EndRead(ar);
        }

        public IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
            return Stream.BeginWrite(buffer, offset, count, callback, state);
        }

        public void EndWrite(IAsyncResult ar) {
            Stream.EndWrite(ar);
        }

        public void Dispose() {
            Stream.Dispose();
        }
    }
}
