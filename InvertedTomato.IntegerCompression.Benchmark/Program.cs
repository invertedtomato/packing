using System;
using System.IO;

namespace InvertedTomato.IntegerCompression.Benchmark {
    class Program {
        static void Main(string[] args) {
            using (var writer = new StreamWriter("output.csv", false)) {
                writer.WriteLine("value,min,vlq(1),vlq(2),vlq(3),vlq(4),elias-omega,elias-gamma");
                for (ulong i = 1; i < ulong.MaxValue / 10; i *= 2) {
                    writer.Write(i);
                    writer.Write(",");
                    writer.Write((float)Bits.CountUsed(i) / (float)8);
                    writer.Write(",");
                    writer.Write((float)VLQUnsignedWriter.CalculateBitLength(1, i) / (float)8);
                    writer.Write(",");
                    writer.Write((float)VLQUnsignedWriter.CalculateBitLength(2, i) / (float)8);
                    writer.Write(",");
                    writer.Write((float)VLQUnsignedWriter.CalculateBitLength(3, i) / (float)8);
                    writer.Write(",");
                    writer.Write((float)VLQUnsignedWriter.CalculateBitLength(4, i) / (float)8);
                    writer.Write(",");
                    writer.Write((float)EliasOmegaUnsignedWriter.CalculateBitLength(false, i) / (float)8);
                    writer.Write(",");
                    writer.Write((float)GuessEliasGammaLength(i) / (float)8);
                    writer.WriteLine();
                }
            }
        }

        private static int GuessEliasGammaLength(ulong value) {
            return Bits.CountUsed(value) * 2 - 1;
        }
    }
}
