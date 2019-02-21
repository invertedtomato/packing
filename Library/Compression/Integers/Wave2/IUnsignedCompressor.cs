using System;
using System.IO;

namespace InvertedTomato.Compression.Integers.Wave2 {
	public interface IUnsignedCompressor {
		Int32 CompressUnsigned(Stream output, params UInt64[] symbols);
		Int32 CompressSigned(Stream output, params Int64[] symbols);
	}
}