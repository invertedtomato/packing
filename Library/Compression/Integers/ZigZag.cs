using System;

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    /// Encode signed values as unsigned using ProtoBuffer ZigZag bijection encoding algorithm. https://developers.google.com/protocol-buffers/docs/encoding
    /// </summary>
    public static class ZigZag {
        /// <summary>
        /// Encode a signed long into an ZigZag unsigned long.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ulong Encode(long value) {
            return (ulong)((value << 1) ^ (value >> 63));
        }

        /// <summary>
        /// Decode a ZigZag unsigned long back into a signed long. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long Decode(ulong value) {
            var casted = (long)value;
            return (casted >> 1) ^ (-(casted & 1));
        }
    }
}
