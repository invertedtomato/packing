namespace InvertedTomato {
    public static class ByteExtensions {
        /// <summary>
        /// Get a bit from a byte. Positions 0-7.
        /// </summary>
        public static bool GetBit(this byte value, byte pos) {
            return (value & (1 << pos)) != 0;
        }
    }
}
