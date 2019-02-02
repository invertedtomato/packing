using System;
using InvertedTomato.IO.Buffers;

namespace InvertedTomato {
	public static class ULongBufferExtensions {
		public static void Seed(this Buffer<UInt64> target, UInt64 start, UInt64 end) {
			for (var i = start; i <= end; i++) {
				target.Enqueue(i);
			}
		}
	}
}