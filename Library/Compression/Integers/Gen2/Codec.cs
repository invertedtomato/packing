using System;
using System.IO;
using System.Text;

namespace InvertedTomato.Compression.Integers.Gen2 {
	public abstract class Codec {
		public abstract void EncodeMany(IByteWriter output, UInt64[] values, Int32 offset, Int32 count);

		public abstract void DecodeMany(IByteReader input, UInt64[] values, Int32 offset, Int32 count);

		public abstract Int32? CalculateEncodedBits(UInt64 value);


		public void EncodeMany(Stream output, UInt64[] values) {
			EncodeMany(new StreamWrapper(output), values);
		}

		public void DecodeMany(Stream output, UInt64[] values) {
			DecodeMany(new StreamWrapper(output), values);
		}

		public void EncodeMany(Stream output, UInt64[] values, Int32 offset, Int32 count) {
			EncodeMany(new StreamWrapper(output), values, offset, count);
		}

		public void DecodeMany(Stream output, UInt64[] values, Int32 offset, Int32 count) {
			DecodeMany(new StreamWrapper(output), values, offset, count);
		}

		public void EncodeMany(IByteWriter output, UInt64[] values) {
			EncodeMany(output, values, 0, values.Length);
		}

		public void DecodeMany(IByteReader input, UInt64[] values) {
			DecodeMany(input, values, 0, values.Length);
		}

		public virtual void EncodeSingle(IByteWriter output, UInt64 value) {
			EncodeMany(output, new UInt64[] {value}, 0, 1);
		}

		public virtual UInt64 DecodeSingle(IByteReader input) {
			var v = new UInt64[1];
			DecodeMany(input, v, 0, 1);
			return v[0];
		}

		public void EncodeSingle(Stream output, UInt64 value) {
			EncodeSingle(new StreamWrapper(output), value);
		}

		public UInt64 DecodeSingle(Stream input) {
			return DecodeSingle(new StreamWrapper(input));
		}
	}
}