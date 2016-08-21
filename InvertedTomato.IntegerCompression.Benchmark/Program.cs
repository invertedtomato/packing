using InvertedTomato.IO;
using System;
using System.IO;

namespace InvertedTomato.IntegerCompression.Benchmark {
    class Program {
        static void Main(string[] args) {
            using (var writer = new StreamWriter("output.csv", false)) {
                writer.WriteLine("value,min,vlq,elias-omega,elias-gamma,tweedale,thompson(32),thompson(64)");
                for (ulong i = 1; i < ulong.MaxValue / 10; i *= 2) {
                    writer.Write(i);
                    writer.Write(",");
                    writer.Write(Bits.CountUsed(i));
                    writer.Write(",");
                    writer.Write(VLQUnsignedWriter.CalculateBitLength(0, i));
                    writer.Write(",");
                    writer.Write(EliasOmegaUnsignedWriter.CalculateBitLength( i));
                    writer.Write(",");
                    writer.Write(GuessEliasGammaLength(i));
                    writer.Write(",");
                    writer.Write(GuessTweedaleLength(i));
                    writer.Write(",");
                    writer.Write(GuessThompsonLength32(i));
                    writer.Write(",");
                    writer.Write(GuessThompsonLength64(i));
                    writer.WriteLine();
                }
            }
        }

        private static int GuessEliasGammaLength(ulong value) {
            return Bits.CountUsed(value) * 2 - 1;
        }

        private static int GuessTweedaleLength(ulong value) {
            var headerBits = 6;
            var valueBits = Bits.CountUsed(value);

            return headerBits + valueBits;
        }

        private static int GuessThompsonLength32(ulong v) {
            var headerBits = 5;

            if(v > uint.MaxValue) {
                return 0;
            }
            var l = (ulong)Bits.CountUsed(v) - 1; // No need for the MSB

            var valueBits = Bits.CountUsed(v) - 1; // No need for the MSB

            return headerBits + valueBits;

            /* ENCODE:
             * Let v be the value to be encoded (max 64 bits)
             * Let l be the number of bits in v, minus 1
             * Write l as 6-bits
             * Write v except the MSB
             */
        }
        private static int GuessThompsonLength64(ulong v) {
            var headerBits = 6;

            var l = (ulong)Bits.CountUsed(v) - 1; // No need for the MSB

            var valueBits = Bits.CountUsed(v)-1 ; // No need for the MSB

            return headerBits + valueBits ;

            /* ENCODE:
             * Let v be the value to be encoded (max 64 bits)
             * Let l be the number of bits in v, minus 1
             * Write l as 6-bits
             * Write v except the MSB
             */
        }
    }
}
