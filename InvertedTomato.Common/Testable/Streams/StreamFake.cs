using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InvertedTomato.Testable.Streams {
    public sealed class StreamFake : IStream {
        private readonly AutoResetEvent InputLock = new AutoResetEvent(false);

        private readonly MemoryStream Output = new MemoryStream();
        private readonly MemoryStream Input = new MemoryStream();


        public bool IsDisposed { get; private set; }

        public void QueueInput(byte[] raw) {
            if (null == raw) {
                throw new ArgumentNullException("raw");
            }

            // Add to input queue
            Input.SetLength(0);
            Input.Write(raw);
            Input.Seek(0, SeekOrigin.Begin);

            // Release input lock
            InputLock.Set();

            // Wait
            Thread.Sleep(200);
        }

        public byte[] ReadOutput() {
            // Wait
            Thread.Sleep(200);

            // Fetch
            Output.Rewind();
            return Output.ToArray();
        }


        public IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
            if (IsDisposed) {
                throw new ObjectDisposedException("Object disposed.");
            }

            return Task.Run(() => {
                // Wait for data
                if (Input.Position == Input.Length) {
                    InputLock.WaitOne();
                }

                // Simulate read
                var result = Input.ReadAsync(buffer, offset, count);

                callback(result);
            });
        }
        public int EndRead(IAsyncResult ar) {
            var result = (Task<int>)ar;
            return result.Result;
        }
        public IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) {
            if (IsDisposed) {
                throw new ObjectDisposedException("Object disposed.");
            }

            return Task.Run(() => {
                Thread.Sleep(100);

                // Simulate write
                var result = Output.WriteAsync(buffer, offset, count);

                callback(result);
            });
        }
        

        public void EndWrite(IAsyncResult ar) {}

        public void Dispose() {
            IsDisposed = true;
        }
    }
}
