using InvertedTomato.IO;
using System;
using System.IO;

namespace InvertedTomato.IntegerCompression.Benchmark {
    class Program {
        static void Main(string[] args) {
            using (var writer = new StreamWriter("output.csv", false)) {
                writer.WriteLine(",min,elias-omega,elias-gamma,fibonacci,vlq(7),vlq(10),thompson-alpha(4),thompson-alpha(5),thompson-alpha(6)");
                for (ulong i = 1; i < ulong.MaxValue / 10; i *= 10) {
                    writer.Write(i);
                    writer.Write(",");
                    writer.Write(Bits.CountUsed(i));
                    writer.Write(",");
                    writer.Write(EliasOmegaUnsignedWriter.CalculateBitLength(i));
                    writer.Write(",");
                    writer.Write(EliasGammaUnsignedWriter.CalculateBitLength(i));
                    writer.Write(",");
                    writer.Write(FibonacciUnsignedWriter.CalculateBitLength(i));
                    writer.Write(",");
                    writer.Write(VLQUnsignedWriter.CalculateBitLength(7, i));
                    writer.Write(",");
                    writer.Write(VLQUnsignedWriter.CalculateBitLength(10, i));
                    writer.Write(",");
                    writer.Write(ThompsonAlphaUnsignedWriter.CalculateBitLength(4, i));
                    writer.Write(",");
                    writer.Write(ThompsonAlphaUnsignedWriter.CalculateBitLength(5, i));
                    writer.Write(",");
                    writer.Write(ThompsonAlphaUnsignedWriter.CalculateBitLength(6, i));
                    writer.WriteLine();
                }
            }
        }
    }
}
