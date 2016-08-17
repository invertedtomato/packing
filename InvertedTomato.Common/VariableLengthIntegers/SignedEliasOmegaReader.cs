using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
    public class SignedEliasOmegaReader : ISignedReader {
        public static IEnumerable<long> ReadAll(byte[] input) {
            return ReadAll(false, input);
        }

        public static IEnumerable<long> ReadAll(bool allowZeros, byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new SignedEliasOmegaReader(stream, allowZeros)) {
                    long value;
                    while (reader.TryRead(out value)) {
                        yield return value;
                    }
                }
            }
        }


        public bool IsDisposed { get; private set; }
        private readonly UnsignedEliasOmegaReader Underlying;

        public SignedEliasOmegaReader(Stream input) {
            Underlying = new UnsignedEliasOmegaReader(input);
        }
        public SignedEliasOmegaReader(Stream input, bool allowZero) {
            Underlying = new UnsignedEliasOmegaReader(input, allowZero);
        }

        public bool TryRead(out long value) {
            ulong innerValue;
            var success = Underlying.TryRead(out innerValue);
            value = ZigZag.Decode(innerValue);
            return success;
        }

        public long Read() {
            long value;
            if (!TryRead(out value)) {
                throw new EndOfStreamException();
            }
            return value;
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
