using System;
using System.IO;

namespace InvertedTomato.Compression.Integers {
	public interface IUnsignedCompressor {
		void CompressUnsigned(Stream output, params UInt64[] symbols);
		void CompressSigned(Stream output, params Int64[] symbols);
	}
}