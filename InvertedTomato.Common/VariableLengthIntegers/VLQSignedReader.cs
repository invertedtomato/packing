using InvertedTomato.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
    /// <summary>
    /// Utility to encode and decode signed numbers to the smallest possible number of raw bytes.
    /// </summary>
    public class VLQSignedReader : ISignedReader {
        public static IEnumerable<long> ReadAll(byte[] input) {
            return ReadAll(1, input);
        }
        public static IEnumerable<long> ReadAll(int minBytes, byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                using (var reader = new VLQSignedReader(stream, minBytes)) {
                    long value;
                    while (reader.TryRead(out value)) {
                        yield return value;
                    }
                }
            }
        }


        public bool IsDisposed { get; private set; }
        private readonly VLQUnsignedReader Underlying;

        public VLQSignedReader(Stream input) {
            Underlying = new VLQUnsignedReader(input);
        }
        public VLQSignedReader(Stream input, int minBytes) {
            Underlying = new VLQUnsignedReader(input, minBytes);
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