using System;
using System.Diagnostics;

namespace InvertedTomato.Compression.Integers.Gen2.Wrappers {
	public class ByteArraySegmentWrapper : IByteReader { // Based on work by Vicente Penades
		public ByteArraySegmentWrapper(Byte[] underlying) {
			if (null == underlying) {
				throw new ArgumentNullException(nameof(underlying));
			}

			Underlying = new (underlying);
		}

		public ByteArraySegmentWrapper(ArraySegment<Byte> underlying) {
			Underlying = underlying;
		}

		private ArraySegment<Byte> Underlying;

		public Byte ReadByte() {
			// Extract value
			Debug.Assert(Underlying.Array != null, "Underlying.Array != null");
			var value = Underlying.Array![Underlying.Offset];

			// Advance pointer
			Underlying = new (Underlying.Array, Underlying.Offset + 1, Underlying.Count - 1);

			return value;
		}
	}
}