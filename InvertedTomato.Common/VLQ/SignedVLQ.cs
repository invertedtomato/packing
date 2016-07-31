using System;

namespace InvertedTomato.VLQ {
    public sealed class SignedVLQ {
        /// <summary>
        /// Encode integer as signed VLQ.
        /// </summary>
        public static byte[] Encode(long value) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Has enough bytes been provided. If false more bytes need to be added using AppendByte before a value is available.
        /// </summary>
        public bool IsComplete { get; private set; }
        
        /// <summary>
        /// Output parameters
        /// </summary>
        private long Value;
        private byte Position;
        private bool IsPositive;
        
        /// <summary>
        /// Append a byte to the VLQ. Returns true if all bytes are accounted for and the value is ready for reading.
        /// </summary>
        public bool AppendByte(byte value) {
            if (IsComplete) {
                throw new InvalidOperationException("Value already complete.");
            }

            byte InputPosition = 0;

            // Handle sign
            if (Position == 0) {
                IsPositive = !value.GetBit(0);
                InputPosition++;
            }

            // Add value
            for (var i = InputPosition; InputPosition < 7; InputPosition++) {
                if (value.GetBit(InputPosition)) {
                    if (IsPositive) {
                        Value += 1 << Position;
                    } else {
                        Value -= 1 << Position;
                    }
                }

                Position++;
            }

            // Determine if complete
            return IsComplete = value.GetBit(7);
        }

        /// <summary>
        /// Convert value to a signed integer.
        /// </summary>
        /// <returns></returns>
        public long ToInt64() {
            if (!IsComplete) {
                throw new InvalidOperationException("Value not complete.");
            }
            if (Value > long.MaxValue) {
                throw new OverflowException("Value too large for UInt64.");
            }

            return IsPositive ? -1 * Value : Value;
        }
    }
}
