using System;
using System.Collections.Generic;
using System.IO;

namespace InvertedTomato.Compression.Integers {
	public class ListByteWrapper : IByteWriter {
		private readonly IList<Byte> Underlying;

		public ListByteWrapper(IList<Byte> underlying) {
			if (null == underlying) {
				throw new ArgumentNullException(nameof(underlying));
			}

			Underlying = underlying;
		}

		public void WriteByte(Byte value) {
			Underlying.Add(value);
		}
	}
}