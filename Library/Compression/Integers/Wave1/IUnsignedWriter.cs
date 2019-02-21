using System;

namespace InvertedTomato.Compression.Integers.Wave1 {
	public interface IUnsignedWriter : IDisposable {
		void Write(UInt64 value);

		// static byte[] Write (params ulong value);
		// static byte[] Write (IEnumerable<ulong> values);
	}
}