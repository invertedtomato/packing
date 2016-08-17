using System;
using System.IO;

namespace InvertedTomato.VariableLengthIntegers.Benchmark {
    class Program {
        static void Main(string[] args) {
            using (var writer = new StreamWriter("output.csv", false)) {
                writer.WriteLine("value,vlq,elias-omega");
                for (ulong i = 1; i < 100000; i++) {
                    writer.Write(i);
                    writer.Write(",");
                    writer.Write(VLQUnsignedWriter.CalculateBitLength(1, i));
                    writer.Write(",");
                    writer.Write(EliasOmegaUnsignedWriter.CalculateBitLength(false, i));
                    writer.WriteLine();
                }
            }
        }
    }
}
