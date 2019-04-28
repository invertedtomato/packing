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
					writer.Write(new EliasOmegaCodec().CalculateEncodedBits(i));
					writer.Write(",");
					writer.Write(new EliasGammaCodec().CalculateEncodedBits(i));
					writer.Write(",");
					writer.Write(new EliasDeltaCodec().CalculateEncodedBits(i));
					writer.Write(",");
					writer.Write(new FibonacciCodec().CalculateEncodedBits(i));
					writer.Write(",");
					writer.Write(new VlqCodec().CalculateEncodedBits( i));
					writer.Write(",");
					writer.Write(new ThompsonAlphaCodec().CalculateEncodedBits(i));
					writer.WriteLine();
				}
			}
		}
	}
}