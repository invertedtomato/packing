using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers {
    public class SignedOmegaReader : IIntegerReader<long> {
        public static IEnumerable<long> ReadAll(byte[] input) {
            return ReadAll(false, input);
        }

        public static IEnumerable<long> ReadAll(bool allowZeros, byte[] input) {
            if (null == input) {
                throw new ArgumentNullException("input");
            }

            using (var stream = new MemoryStream(input)) {
                var reader = new SignedOmegaReader(stream, allowZeros);

                long value;
                while (reader.TryRead(out value)) {
                    yield return value;
                }
            }
        }

        private readonly UnsignedOmegaReader Underlying;

        public SignedOmegaReader(Stream input) {
            Underlying = new UnsignedOmegaReader(input);
        }
        public SignedOmegaReader(Stream input, bool allowZero) {
            Underlying = new UnsignedOmegaReader(input, allowZero);
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
    }
}
