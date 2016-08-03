namespace InvertedTomato {
    public static class ByteExtensions {
        /// <summary>
        /// Get a bit from a byte. Positions 0-7.
        /// </summary>
        public static bool GetBit(this byte value, int position) {
            return (value & (1 << position)) != 0;
        }

        /* This doesn't work in an extension method
        /// <summary>
        /// Set a bit on a byte. Positions 0-7
        /// </summary>
        public static void SetBit(this byte value, int position) {
            value |= (byte)(1 << position);
        }

        /// <summary>
        /// Clear a bit on a byte. Positions 0-7.
        /// </summary>
        public static void ClearBit(this byte value, int position) {
            value &= (byte)~(1 << position);
        }*/
    }
}
