using System;
using System.IO;

namespace InvertedTomato.VLQ {
    public class UnsignedVLQ {
        /// <summary>
        /// Encode integer as unsigned VLQ.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] Encode(ulong value) {
            throw new NotImplementedException();
        }
        public static void Encode(long value, Stream output) {
            if (null == output) {
                throw new ArgumentNullException("output");
            }

            throw new NotImplementedException();
        }
        public static ulong Decode(Stream stream) {
            ulong result = 0;
            int position = 0;

            while (true) {
                var b = (byte)stream.ReadByte();

                for (byte i = 0; i < 7; i++) {
                    if (b.GetBit(i)) {
                        result += 1UL << position;
                    }
                    position++;
                }
                if (b.GetBit(7)) {
                    return result;
                }
            }
        }

        /// <summary>
        /// Is there more bytes remaining
        /// </summary>
        private bool IsMore = true;

        /// <summary>
        /// Output parameters
        /// </summary>
        private ulong Value;
        private byte Position;

        /// <summary>
        /// Append a byte to the VLQ. Returns true if all bytes are accounted for and the value is ready for reading.
        /// </summary>
        public bool AppendByte(byte value) {
            if (!IsMore) {
                throw new InvalidOperationException("Value already complete.");
            }

            // Add value
            for (byte i = 0; i < 7; i++) {
                if (value.GetBit(i)) {
                    checked { // Recieved more bits than can fit in an int64 - throw an exception instead of wrapping
                        Value += 1UL << Position;
                    }
                }

                Position++;
            }

            // Determine if complete
            IsMore = value.GetBit(7);

            return IsMore;
        }

        /// <summary>
        /// Convert value to an unsigned integer.
        /// </summary>
        /// <returns></returns>
        public ulong ToValue() {
            if (IsMore) {
                throw new InvalidOperationException("Value not complete.");
            }

            return Value;
        }
    }
}
