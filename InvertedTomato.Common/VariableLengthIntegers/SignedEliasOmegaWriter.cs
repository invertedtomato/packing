using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
    public class SignedEliasOmegaWriter : ISignedWriter, IDisposable {
        public static byte[] WriteAll(IEnumerable<long> values) { return WriteAll(false, values); }

        public static byte[] WriteAll(bool allowZeros, IEnumerable<long> values) {
            using (var stream = new MemoryStream()) {
                using (var writer = new SignedEliasOmegaWriter(stream, allowZeros)) {
                    foreach (var value in values) {
                        writer.Write(value);
                    }
                }
                return stream.ToArray();
            }
        }

        public bool IsDisposed { get; private set; }
        private readonly UnsignedEliasOmegaWriter Underlying;

        public SignedEliasOmegaWriter(Stream output) {
            Underlying = new UnsignedEliasOmegaWriter(output);
        }
        public SignedEliasOmegaWriter(Stream input, bool allowZero) {
            Underlying = new UnsignedEliasOmegaWriter(input, allowZero);
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
