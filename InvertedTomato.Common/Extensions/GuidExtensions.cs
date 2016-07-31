using System;

namespace InvertedTomato {
	public static class GuidExtensions {
        /// <summary>
        /// Convert to a short text representation of Guid, loosely based on Base64. Use GuidUtility.FromShort() to reverse.
        /// </summary>
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