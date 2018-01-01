using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.Compression.Integers {
    public class RawCodec : Codec {
        public static readonly UInt64 MinValue = UInt64.MinValue;
        public static readonly UInt64 MaxValue = UInt64.MaxValue;

        public override void CompressUnsigned(Stream output, params UInt64[] symbols) {
#if DEBUG
            if(null == output) {
                throw new ArgumentNullException(nameof(output));
            }
            if(null == symbols) {
                throw new ArgumentNullException(nameof(symbols));
            }
#endif

            foreach(var symbol in symbols) {
                // Convert to raw byte array
                var raw = BitConverter.GetBytes(symbol);

                // Add to output
                output.Write(raw, 0, 8);
            }
        }

        public override IEnumerable<UInt64> DecompressUnsigned(Stream input, Int32 count) {
#if DEBUG
            if(null == input) {
                throw new ArgumentNullException(nameof(input));
            }
            if(count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
#endif

            for(var i = 0; i < count; i++) {
                // Get next 8 bytes
                var buffer = new Byte[8];
                try {
                    if(input.Read(buffer, 0, 8) != 8) {
                        throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
                    }
                } catch(ArgumentException) {
                    throw new EndOfStreamException("Input ends with a partial symbol. More bytes required to decode.");
                }

                // Convert to symbol
                var symbol = BitConverter.ToUInt64(buffer, 0);

                // Return symbol
                yield return symbol;
            }
        }
    }
}
