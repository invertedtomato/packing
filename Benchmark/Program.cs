using System;
using System.IO;
using InvertedTomato.IO;

namespace InvertedTomato.Compression.Integers.Benchmark {
	internal class Program {
		private static void Main(String[] args) {
			using (var writer = new StreamWriter("output.csv", false)) {
				writer.WriteLine(",min,elias-omega,elias-gamma,elias-delta,fibonacci,vlq(7),thompson-alpha(6)");
				for (UInt64 i = 1; i < UInt64.MaxValue / 10; i *= 2) {
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