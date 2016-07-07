using System;
using System.Text;

namespace InvertedTomato {
	public static class ByteArrayExtensions {
		/// <summary>
		/// Byte comparison with another array.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="other"></param>
		/// <returns></returns>
		public static bool Equals(this byte[] target, byte[] other) {
			return ArrayUtility.Compare(target, other);
		}

		/// <summary>
		/// Convert to string with default encoding.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public static string ToStringDefaultEncoding(this byte[] target) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}

			return System.Text.Encoding.Default.GetString(target);
		}

		/// <summary>
		/// Convert to hex string.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public static string ToHexString(this byte[] target) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}

			return BitConverter.ToString(target).Replace("-", String.Empty);
		}

		/// <summary>
		/// Convert to Base64 string.
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public static string ToBase64String(this byte[] target) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}

			return Convert.ToBase64String(target);
		}

		/// <summary>
		/// Find first occurrence of needle in array.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="needle"></param>
		/// <param name="startIndex"></param>
		/// <returns></returns>
		public static int IndexOf(this byte[] target, byte needle, int startIndex) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}

			return Array.IndexOf(target, needle, startIndex);
		}

		public static ulong ToUInt64(this byte[] target) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}

			ulong value = BitConverter.ToUInt64(target, 0);
			return value;
		}

		public static uint ToUInt32(this byte[] target) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}

			uint value = BitConverter.ToUInt32(target, 0);
			return value;
		}

		public static ushort ToUInt16(this byte[] target) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}

			ushort value = BitConverter.ToUInt16(target, 0);
			return value;
		}
	}
}
