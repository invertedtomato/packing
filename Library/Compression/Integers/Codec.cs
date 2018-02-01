using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace InvertedTomato.Compression.Integers {
    public abstract class Codec : IUnsignedCompressor, IUnsignedDecompressor {
        public abstract void CompressUnsigned(Stream output, params UInt64[] symbols);

        public void CompressUnsigned(Stream output, params Int64[] symbols) {
#if DEBUG
            if(null == output) {
                throw new ArgumentNullException(nameof(output));
            }
            if(null == symbols) {
                throw new ArgumentNullException(nameof(symbols));
            }
#endif

            CompressUnsigned(output, symbols.Select(symbol => {
                if(symbol < 0) {
                    throw new ArgumentOutOfRangeException("symbols");
                }
                return (UInt64)symbol;
            }).ToArray());
        }

        public void CompressSigned(Stream output, params Int64[] symbols) {
#if DEBUG
            if(null == output) {
                throw new ArgumentNullException(nameof(output));
            }
            if(null == symbols) {
                throw new ArgumentNullException(nameof(symbols));
            }
#endif

            CompressUnsigned(output, symbols.Select(symbol => ZigZag.Encode(symbol)).ToArray());
        }


        public MemoryStream CompressUnsigned(params UInt64[] symbols) {
#if DEBUG
            if(null == symbols) {
                throw new ArgumentNullException(nameof(symbols));
            }
#endif

            var output = new MemoryStream();
            CompressUnsigned(output, symbols);
            output.Seek(0, SeekOrigin.Begin);
            return output;
        }

        public MemoryStream CompressUnsigned(params Int64[] symbols) {
#if DEBUG
            if(null == symbols) {
                throw new ArgumentNullException(nameof(symbols));
            }
#endif

            var output = new MemoryStream();
            CompressUnsigned(output, symbols);
            output.Seek(0, SeekOrigin.Begin);
            return output;
        }

        public MemoryStream CompressSigned(params Int64[] symbols) {
#if DEBUG
            if(null == symbols) {
                throw new ArgumentNullException(nameof(symbols));
            }
#endif

            var output = new MemoryStream();
            CompressSigned(output, symbols);
            output.Seek(0, SeekOrigin.Begin);
            return output;
        }


        public abstract IEnumerable<UInt64> DecompressUnsigned(Stream input, Int32 count);

        public IEnumerable<Int64> DecompressSigned(Stream input, Int32 count) {
#if DEBUG
            if(null == input) {
                throw new ArgumentNullException(nameof(input));
            }
            if(count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
#endif

            return DecompressUnsigned(input, count).Select(symbol => ZigZag.Decode(symbol));
        }
    }
}
