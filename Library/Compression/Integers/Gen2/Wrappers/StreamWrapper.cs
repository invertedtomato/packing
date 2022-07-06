using System;
using System.IO;

namespace InvertedTomato.Compression.Integers.Gen2 {
	public class StreamWrapper : IByteReader, IByteWriter {
		private readonly Stream Underlying;

		public StreamWrapper(Stream underlying) {
			if (null == underlying) {
				throw new ArgumentNullException(nameof(underlying));
			}

			Underlying = underlying;
		}

		public Byte ReadByte() {
			var b = Underlying.ReadByte();
			if (b == -1) {
				throw new EndOfStreamException();
			}

			return (Byte) b;
		}

		public void WriteByte(Byte value) {
			Underlying.WriteByte(value);
		}
	}
}