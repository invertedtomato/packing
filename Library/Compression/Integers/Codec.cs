using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace InvertedTomato.Compression.Integers {
	public abstract class Codec : IUnsignedCompressor, IUnsignedDecompressor {
		public abstract Int32 CompressUnsigned(Stream output, params UInt64[] value);

		public void CompressUnsigned(Stream output, params Int64[] values) { // Int64 is NOT an accident - this handles to casting
#if DEBUG
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}
#endif

			CompressUnsigned(output, values.Select(symbol => {
				if (symbol < 0) {
					throw new ArgumentOutOfRangeException("symbols");
				}

				return (UInt64) symbol;
			}).ToArray());
		}
		
		public MemoryStream CompressUnsigned(params UInt64[] values) {
#if DEBUG
			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}
#endif

			var output = new MemoryStream();
			CompressUnsigned(output, values);
			output.Seek(0, SeekOrigin.Begin);
			return output;
		}
		
		public Int32 CompressSigned(Stream output, params Int64[] values) {
#if DEBUG
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}
#endif

			return CompressUnsigned(output, values.Select(ZigZag.Encode).ToArray());
		}

		public MemoryStream CompressSigned(params Int64[] values) {
#if DEBUG
			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}
#endif

			var output = new MemoryStream();
			CompressSigned(output, values);
			output.Seek(0, SeekOrigin.Begin);
			return output;
		}


		public abstract Task<Int32> CompressUnsignedAsync(Stream output, params UInt64[] value);

		public async Task<Int32> CompressSignedAsync(Stream output, params Int64[] values) {
#if DEBUG
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}
#endif

			return await CompressUnsignedAsync(output, values.Select(ZigZag.Encode).ToArray());
		}

		public async Task<MemoryStream> CompressUnsignedAsync(params UInt64[] values) {
#if DEBUG
			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}
#endif

			var output = new MemoryStream();
			await CompressUnsignedAsync(output, values);
			output.Seek(0, SeekOrigin.Begin);
			return output;
		}

		public async Task<MemoryStream> CompressSignedAsync(params Int64[] values) {
#if DEBUG
			if (null == values) {
				throw new ArgumentNullException(nameof(values));
			}
#endif

			var output = new MemoryStream();
			await CompressSignedAsync(output, values);
			output.Seek(0, SeekOrigin.Begin);
			return output;
		}


		public abstract Int32 DecompressUnsigned(Stream input, ref UInt64[] output, Int32 offset, Int32 count);

		public Int32 DecompressUnsigned(Stream input, ref UInt64[] output) {
			return DecompressUnsigned(input, ref output, 0, output.Length);
		}

		public Int32 DecompressUnsigned(Stream input, out UInt64 output) {
			var innerOutput = new UInt64[1];
			var used = DecompressUnsigned(input, ref innerOutput);
			output = innerOutput[0];
			return used;
		}
		
		public IEnumerable<UInt64> DecompressUnsigned(Stream input, Int32 count) {
			var output = new UInt64[count];
			DecompressUnsigned(input, ref output, 0, count);
			return output;
		}

		public IEnumerable<Int64> DecompressSigned(Stream input, Int32 count) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			if (count < 0) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
#endif

			return DecompressUnsigned(input, count).Select(ZigZag.Decode);
		}

		
		public abstract Task<IEnumerable<UInt64>> DecompressUnsignedAsync(Stream input, Int32 count);

		public async Task<IEnumerable<Int64>> DecompressSignedAsync(Stream input, Int32 count) {
#if DEBUG
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			if (count < 0) {
				throw new ArgumentOutOfRangeException(nameof(count));
			}
#endif

			return (await DecompressUnsignedAsync(input, count)).Select(symbol => ZigZag.Decode(symbol));
		}


		public abstract Int32 CalculateBitLength(UInt64 value);
	}
}