namespace InvertedTomato.VariableLengthIntegers {
    public static class Bits {
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
