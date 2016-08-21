using System;

namespace InvertedTomato.IntegerCompression {
    public static class Bits {
        [Obsolete("Use InvertedTomato.IO.Bits instead")]
        public static byte CountUsed(ulong value) {
            byte bits = 0;

            do {
                bits++;
                value >>= 1;
            } while (value > 0);

            return bits;
        }
    }
}
