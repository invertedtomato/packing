using System;
using System.Collections.Generic;

namespace InvertedTomato {
	public static class ListExtensions {
		public static void Seed(this List<UInt64> target, UInt64 start, UInt64 end) {
			for (var i = start; i <= end; i++) {
				target.Add(i);
			}
		}
	}
}