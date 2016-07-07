using System;

namespace InvertedTomato {
	public static class ArrayExtensions {
		/// <summary>
		/// Find first occurrence of needle in array.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="needle"></param>
		/// <returns></returns>
		public static int IndexOf<T>(this T[] target, T needle, int startIndex) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}

			return Array.IndexOf(target, needle, startIndex);
		}

		/// <summary>
		/// Find starting index of a sub-array.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="start"></param>
		/// <param name="count"></param>
		/// <param name="needle"></param>
		/// <returns></returns>
		public static int IndexOf(this byte[] target, int start, int count, byte[] needle) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}
			if (null == needle) {
				throw new ArgumentNullException("needle");
			}


			for (int i = start; i < start + count - needle.Length + 1; i++) {
				// Check if needle matches
				var mismatch = false;
				for (int j = 0; j < needle.Length; j++) {
					if (target[i + j] != needle[j]) {
						mismatch = true;
						break;
					}
				}

				// Return success
				if (!mismatch) {
					return i;
				}
			}

			return -1;
		}


		/// <summary>
		/// Extract a subset of the array
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="target"></param>
		/// <param name="index"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static T[] SubArray<T>(this T[] target, int index, int length) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}

			T[] result = new T[length];
			Array.Copy(target, index, result, 0, length);
			return result;
		}

		public static T[] SubArray<T>(this T[] target, int index) {
			return target.SubArray(index, target.Length - index);
		}
	}
}