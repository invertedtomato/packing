using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvertedTomato.VariableLengthIntegers {
    public class SignedVLQWriter : ISignedWriter {
        public static byte[] WriteAll(IEnumerable<long> values) { return WriteAll(1, values); }

        public static byte[] WriteAll(int minBytes, IEnumerable<long> values) {
            using (var stream = new MemoryStream()) {
                using (var writer = new SignedVLQWriter(stream, minBytes)) {

                    foreach (var value in values) {
                        writer.Write(value);
                    }

                    return stream.ToArray();
                }
            }
        }


        public bool IsDisposed { get; private set; }
        private readonly UnsignedVLQWriter Underlying;

        public SignedVLQWriter(Stream output) {
            Underlying = new UnsignedVLQWriter(output);
        }
        public SignedVLQWriter(Stream input, int minBytes) {
            Underlying = new UnsignedVLQWriter(input, minBytes);
        }

        public void Write(long value) {
            var innerValue = ZigZag.Encode(value);
            Underlying.Write(innerValue);
        }

        protected virtual void Dispose(bool disposing) {
            if (IsDisposed) {
                return;
            }
            IsDisposed = true;

            Underlying.Dispose();

            if (disposing) {
                // Dispose managed state (managed objects).
            }
        }
        public void Dispose() {
            Dispose(true);
        }
    }
}
