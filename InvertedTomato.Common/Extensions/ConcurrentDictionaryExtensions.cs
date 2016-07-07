using System.Collections.Concurrent;
using System.Diagnostics.Contracts;

namespace InvertedTomato {
	public static class ConcurrentDictionaryExtensions {
		/// <summary>
		/// Try to remove an element from a dictionary, ignoring the result.
		/// </summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="target"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool TryRemove<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> target, TKey key) {
			Contract.Requires(null != target);

			TValue value;
			return target.TryRemove(key, out value);
		}
	}
}