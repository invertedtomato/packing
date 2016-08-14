namespace InvertedTomato.VariableLengthIntegers {
    /// <summary>
    /// Encode values using ProtoBuffer ZigZag encoding. https://developers.google.com/protocol-buffers/docs/encoding
    /// </summary>
    public static class ZigZag {
        public static ulong Encode(long value) {
            return (ulong)((value << 1) ^ (value >> 63));
        }

        public static long Decode(ulong value) {
            var casted = (long)value;
            return (casted >> 1) ^ (-(casted & 1));
        }
    }
}
