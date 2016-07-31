using System;

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

        /// <summary>
        /// Has enough bytes been provided. If false more bytes need to be added using AppendByte before a value is available.
        /// </summary>
        public bool IsComplete { get; private set; }
        
        /// <summary>
        /// Output parameters
        /// </summary>
        private ulong Value;
        private byte Position;

        /// <summary>
        /// Append a byte to the VLQ. Returns true if all bytes are accounted for and the value is ready for reading.
        /// </summary>
        public bool AppendByte(byte value) {
            if (IsComplete) {
                throw new InvalidOperationException("Value already complete.");
            }

            byte InputPosition = 0;
            
            // Add value
            for (var i = InputPosition; InputPosition < 7; InputPosition++) {
                if (value.GetBit(InputPosition)) {
                    Value += 1UL << Position;
                }

                Position++;
            }

            // Determine if complete
            return IsComplete = value.GetBit(7);
        }
        
        /// <summary>
        /// Convert value to an unsigned integer.
        /// </summary>
        /// <returns></returns>
        public ulong ToValue() {
            if (!IsComplete) {
                throw new InvalidOperationException("Value not complete.");
            }

            return Value;
        }
    }
}
