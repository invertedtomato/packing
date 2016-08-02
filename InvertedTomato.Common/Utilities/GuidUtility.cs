using System;
using System.Security.Cryptography;
using System.Text;

namespace InvertedTomato {
	public class GuidUtility {
		/// <summary>
		/// Create a GUID from a given integer. Useful for legacy interfacing.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Guid CreateFrom(int value) {
			var bytes = new byte[16];
			BitConverter.GetBytes(value).CopyTo(bytes, 0);
			return new Guid(bytes);
		}

		/// <summary>
		/// Create a GUID from a given long. Useful for legacy interfacing.
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Guid CreateFrom(long value) {
			var bytes = new byte[16];
			BitConverter.GetBytes(value).CopyTo(bytes, 0);
			return new Guid(bytes);
		}

		/// <summary>
		/// Create a GUID from a given string. Useful for legacy interfacing. (NOTE: Is lossy)
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static Guid CreateFrom(string value) {
			if (null == value) {
				throw new ArgumentNullException("value");
			}

			using (var sha = SHA256Managed.Create()) {
				// Hash string
				var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(value));

				// Squash into GUID
				var bytes = new byte[16];
				Buffer.BlockCopy(hash, 0, bytes, 0, 16);
				return new Guid(bytes);
			}
		}

        /// <summary>
        /// Convert a short representation of a GUID back into a GUID.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
		public static Guid FromShort(string value) {
			if (null == value) {
				throw new ArgumentOutOfRangeException("value");
			}

			// Un-fix unfriendly base64 characters
			var str = value.Replace('-', '+').Replace('_', '=').Replace('~', '/');

			// Convert to bytes
			var bytes = Convert.FromBase64String(str + "==");

			// Convert to GUID
			return new Guid(bytes);
		}
	}
}