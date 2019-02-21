using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.Compression.Integers.Wave2 {
	public interface IUnsignedDecompressor {
		IEnumerable<UInt64> DecompressUnsigned(Stream input, Int32 count);
		IEnumerable<Int64> DecompressSigned(Stream input, Int32 count);
	}
}