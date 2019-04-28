using System;

namespace InvertedTomato.Compression.Integers {
	public class ByteArraySegmentWrapper : IByteReader { // Based on work by Vicente Penades
		public ByteArraySegmentWrapper(Byte[] underlying) {
			if (null == underlying) {
				throw new ArgumentNullException(nameof(underlying));
			}

			Underlying = new ArraySegment<Byte>(underlying);
		}

		public ByteArraySegmentWrapper(ArraySegment<Byte> underlying) {
			if (null == underlying) {
				throw new ArgumentNullException(nameof(underlying));
			}

			Underlying = underlying;
		}

		private ArraySegment<Byte> Underlying;

		public Byte ReadByte() {
			// Extract value
			var value = Underlying.Array[Underlying.Offset];

			// Advance pointer
			Underlying = new ArraySegment<Byte>(Underlying.Array, Underlying.Offset + 1, Underlying.Count - 1);

			return value;
		}
	}
}