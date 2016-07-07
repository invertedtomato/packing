using System;
using System.Net;
using System.Text;

namespace InvertedTomato {
	/// <summary>
	/// EntensionMethods to convert various objects to bytes
	/// </summary>
	public static class GetBytesExtensions {
		/// <summary>
		/// Get address bytes in a requested array size.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static byte[] GetBytes(this IPAddress target, int length) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}

			// Convert to bytes
			var raw = target.GetAddressBytes();
			if (raw.Length > length) {
				throw new ArgumentException("Address is longer than length.", "target");
			}

			// Pad into output
			var output = new byte[length];
			Buffer.BlockCopy(raw, 0, output, output.Length - raw.Length, raw.Length);

			return output;
		}

        /// <summary>
        /// Gets the bytes.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">target</exception>
        /// <exception cref="System.ArgumentException">Value is longer than length.;target</exception>
        public static byte[] GetBytes(this string target, int length) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}

			// Convert to bytes
			var raw = Encoding.ASCII.GetBytes(target);
			if (raw.Length > length) {
				throw new ArgumentException("Value is longer than length.", "target");
			}

			// Pad into output
			var output = new byte[length];
			Buffer.BlockCopy(raw, 0, output, output.Length - raw.Length, raw.Length);

			return output;
		}

		public static byte[] GetBytes(this byte value) {
			return new byte[] { value };
		}

		public static byte[] GetBytes(this sbyte value) {
			return new byte[] { (byte)(value + 128) };
		}

		public static byte[] GetBytes(this ushort value) {
			return BitConverter.GetBytes(value);
		}

		public static byte[] GetBytes(this short value) {
			return BitConverter.GetBytes(value);
		}

		public static byte[] GetBytes(this uint value) {
			return BitConverter.GetBytes(value);
		}

		public static byte[] GetBytes(this int value) {
			return BitConverter.GetBytes(value);
		}

		public static byte[] GetBytes(this long value) {
			return BitConverter.GetBytes(value);
		}

		public static byte[] GetBytes(this ulong value) {
			return BitConverter.GetBytes(value);
		}

		public static byte[] GetBytes(this double value) {
			return BitConverter.GetBytes(value);
		}

		public static byte[] GetBytes(this bool target) {
			return BitConverter.GetBytes(target);
		}

        public static byte[] GetBytes(this float value) {
            return BitConverter.GetBytes(value);
        }
    }
}
