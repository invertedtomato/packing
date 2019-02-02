using System;

namespace InvertedTomato.Compression.Integers {
	public interface ISignedReader : IDisposable {
		Int64 Read();

		// static long ReadOneDefault(byte[] input); 
	}
}