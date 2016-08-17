using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
    public class SignedOmegaWriter : IIntegerWriter<long>, IDisposable {
        public static byte[] WriteAll(IEnumerable<long> values) { return WriteAll(false, values); }

        public static byte[] WriteAll(bool allowZeros, IEnumerable<long> values) {
            using (var stream = new MemoryStream()) {
                var writer = new SignedOmegaWriter(stream, allowZeros);

                foreach (var value in values) {
                    writer.Write(value);
                }

                return stream.ToArray();
            }

        }

        public bool IsDisposed { get; private set; }
        private readonly UnsignedOmegaWriter Underlying;

        public SignedOmegaWriter(Stream output) {
            Underlying = new UnsignedOmegaWriter(output);
        }
        public SignedOmegaWriter(Stream input, bool allowZero) {
            Underlying = new UnsignedOmegaWriter(input, allowZero);
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
