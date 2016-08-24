using System.Collections.Generic;

namespace InvertedTomato.Compression.Integers {
    public static class Fibonacci {
        public static readonly ulong[] Values;
        public static Dictionary<byte, ulong> Values2;

        static Fibonacci() {
            // Compute all Fibonacci numbers that can fit in a ulong
            Values = new ulong[92];
            Values[0] = 1;
            Values[1] = 2;
            for (var i = 2; i < Values.Length; i++) {
                Values[i] = Values[i - 1] + Values[i - 2];
            }
        }
    }
}

