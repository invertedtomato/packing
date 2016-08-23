using InvertedTomato.IO;
using System;
using System.IO;

namespace InvertedTomato.Compression.Integers.Benchmark {
    class Program {
        static void Main(string[] args) {
            using (var writer = new StreamWriter("output.csv", false)) {
                writer.WriteLine(",min,elias-omega,elias-gamma,elias-delta,fibonacci,vlq(7),thompson-alpha(6)");
                for (ulong i = 1; i < ulong.MaxValue / 10; i *= 2) {
                    writer.Write(i);
                    writer.Write(",");
                    writer.Write(Bits.CountUsed(i));
                    writer.Write(",");
                    writer.Write(EliasOmegaUnsignedWriter.CalculateBitLength(i));
                    writer.Write(",");
                    writer.Write(EliasGammaUnsignedWriter.CalculateBitLength(i));
                    writer.Write(",");
                    writer.Write(EliasDeltaUnsignedWriter.CalculateBitLength(i));
                    writer.Write(",");
                    writer.Write(FibonacciUnsignedWriter.CalculateBitLength(i));
                    writer.Write(",");
                    writer.Write(VLQUnsignedWriter.CalculateBitLength(7, i));
                    writer.Write(",");
                    writer.Write(ThompsonAlphaUnsignedWriter.CalculateBitLength(6, i));
                    writer.WriteLine();
                }
            }
        }
    }
}
