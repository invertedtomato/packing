using System;

namespace InvertedTomato {
	public static class GuidExtensions {
		public static string ToShort(this Guid target) {
			// Get bytes
			var bytes = target.ToByteArray();

			// Encode in base64
			var str = Convert.ToBase64String(bytes).Substring(0, 22);

			// Replace unfriendly base64 characters
			str = str.Replace('+', '-').Replace('=', '_').Replace('/', '~');

			return str;
		}
	}
}