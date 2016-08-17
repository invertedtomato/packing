using System;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers.Benchmark {
    class Program {
        static void Main(string[] args) {
            using (var writer = new StreamWriter("output.csv", false)) {
                writer.WriteLine("value,vlq(1),vlq(2),vlq(3),elias-omega,elias-gamma");
                for (ulong i = 1; i < 1024; i++) {
                    var used = Bits.CountUsed(i);

                    writer.Write(i);
                    writer.Write(",");
                    writer.Write(VLQUnsignedWriter.CalculateBitLength(1, i) - used);
                    writer.Write(",");
                    writer.Write(VLQUnsignedWriter.CalculateBitLength(2, i) - used);
                    writer.Write(",");
                    writer.Write(VLQUnsignedWriter.CalculateBitLength(3, i) - used);
                    writer.Write(",");
                    writer.Write(EliasOmegaUnsignedWriter.CalculateBitLength(false, i) - used);
                    writer.Write(",");
                    writer.Write(GuessEliasGammaLength(i) - used);
                    writer.WriteLine();
                }
            }
        }

        private static int GuessEliasGammaLength(ulong value) {
            return Bits.CountUsed(value)*2-1;
        }
    }
}
