using System;

namespace InvertedTomato {
    public class VLQ {
        /// <summary>
        /// Encode a 64-bit signed integer as VLQ.
        /// </summary>
        public static byte[] Encode(long value) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Encode a 64-bit unsigned integer as VLQ.
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
        /// If the VLQ is a signed integer.
        /// </summary>
        public bool IsSigned { get; private set; }

        /// <summary>
        /// Output parameters
        /// </summary>
        private ulong Value;
        private ushort ValuePosition;
        private bool IsNegative;

        /// <summary>
        /// Instantiate a VLQ. The algorithm is varied slightly if signed integers are to be supported. Unsigned and signed bytes cannot be intermixed.
        /// </summary>
        /// <param name="isSigned"></param>
        public VLQ(bool isSigned) {
            IsSigned = IsSigned;
        }

        /// <summary>
        /// Append a byte to the VLQ. Returns true if the last required byte is provided.
        /// </summary>
        public bool AppendByte(byte value) {
            if (IsComplete) {
                throw new InvalidOperationException("Value already complete.");
            }

            byte InputPosition = 0;

            // Handle sign
            if (IsSigned && ValuePosition == 0) {
                IsNegative = !value.GetBit(0);
                InputPosition++;
            }

            // Add value
            for (var i = InputPosition; InputPosition < 7; InputPosition++) {
                if (value.GetBit(InputPosition)) {
                    Value += 1UL << ValuePosition;
                }

                ValuePosition++;
            }

            // Determine if complete
            return IsComplete = value.GetBit(7);
        }

        /// <summary>
        /// Convert value to a 64.bit unsigned integer.
        /// </summary>
        /// <returns></returns>
        public ulong ToUInt64() {
            if (!IsComplete) {
                throw new InvalidOperationException("Value not complete.");
            }
            if (IsSigned) {
                throw new InvalidOperationException("Not supported for signed values.");
            }
            if (Value > ulong.MaxValue) { // Technically not needed, but here for completeness
                throw new OverflowException("Value too large for UInt64.");
            }

            return Value;
        }

        /// <summary>
        /// Convert value to a 64-bit signed integer.
        /// </summary>
        /// <returns></returns>
        public long ToInt64() {
            if (!IsComplete) {
                throw new InvalidOperationException("Value not complete.");
            }
            if (Value > long.MaxValue) {
                throw new OverflowException("Value too large for UInt64.");
            }

            return IsNegative ? -1 * (long)Value : (long)Value;
        }
    }
}
