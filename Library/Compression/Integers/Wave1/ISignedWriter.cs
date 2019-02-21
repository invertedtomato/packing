using System;

namespace InvertedTomato.Compression.Integers.Wave1 {
	public interface ISignedWriter : IDisposable {
		void Write(Int64 value);

		// static byte[] Write (params long value);
		// static byte[] Write (IEnumerable<long> values);
	}
}