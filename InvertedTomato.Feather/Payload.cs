using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato.Feather {
    public abstract class Payload {
        public byte OpCode { get; protected set; }

        public int Length { get { return (int)Inner.Length; } }


        public bool IsDisposed { get; private set; }

        protected MemoryStream Inner;


        public byte[] ToByteArray() {
            return Inner.ToArray();
        }

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            if (disposing) {
                // Dispose managed state (managed objects)
                Inner.DisposeIfNotNull();
            }
        }
        public void Dispose() {
            Dispose(true);
        }
    }
}
