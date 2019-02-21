using System;

namespace InvertedTomato.Compression.Integers.Wave1 {
	public interface ISignedReader : IDisposable {
		Int64 Read();

		// static long ReadOneDefault(byte[] input); 
	}
}