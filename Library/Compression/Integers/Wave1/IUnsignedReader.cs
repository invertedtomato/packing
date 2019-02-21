using System;

namespace InvertedTomato.Compression.Integers.Wave1 {
	public interface IUnsignedReader : IDisposable {
		UInt64 Read();

		// static ulong ReadOneDefault(); 
	}
}