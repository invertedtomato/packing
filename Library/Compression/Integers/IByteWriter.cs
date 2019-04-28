using System;

namespace InvertedTomato.Compression.Integers {
	public interface IByteWriter {
		void WriteByte(Byte value);
	}
}