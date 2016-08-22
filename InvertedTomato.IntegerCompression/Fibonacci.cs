namespace InvertedTomato.IntegerCompression {
    public static class Fibonacci {
        public static readonly ulong[] Values;

        static Fibonacci() {
            // Compute all Fibonacci numbers
            Values = new ulong[92];
            Values[0] = 1;
            Values[1] = 2;
            for (var i = 2; i < Values.Length; i++) {
                Values[i] = Values[i - 1] + Values[i - 2];
            }
        }
    }
}
