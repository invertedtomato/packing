using InvertedTomato.IO;
using System;
using System.IO;

namespace InvertedTomato.IntegerCompression.Benchmark {
    class Program {
        static void Main(string[] args) {
            using (var writer = new StreamWriter("output.csv", false)) {
                writer.WriteLine("value,min,vlq,elias-omega,elias-gamma,thompson-alpha(6)");
                for (ulong i = 1; i < ulong.MaxValue / 10; i *= 2) {
                    writer.Write(i);
                    writer.Write(",");
                    writer.Write(Bits.CountUsed(i));
                    writer.Write(",");
                    writer.Write(VLQUnsignedWriter.CalculateBitLength(0, i));
                    writer.Write(",");
                    writer.Write(EliasOmegaUnsignedWriter.CalculateBitLength(i));
                    writer.Write(",");
                    writer.Write(EliasGammaUnsignedWriter.CalculateBitLength(i));
                    writer.Write(",");
                    writer.Write(ThompsonAlphaUnsignedWriter.CalculateBitLength(6, i));
                    writer.WriteLine();
                }
            }
        }
    }
}
