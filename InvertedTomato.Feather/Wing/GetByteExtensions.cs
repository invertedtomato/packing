using System;
using System.Linq;
using System.Text;

namespace InvertedTomato.Feather {
    /// <summary>
    /// A bunch of feather specific getByte extension methods
    /// </summary>
    public static class GetByteExtensions {
        public static byte[] GetBytes(this byte? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }

        public static byte[] GetBytes(this sbyte? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }

        public static byte[] GetBytes(this ushort? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }
        public static byte[] GetBytes(this short? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }

        public static byte[] GetBytes(this uint? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }

        public static byte[] GetBytes(this int? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }

        public static byte[] GetBytes(this long? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }

        public static byte[] GetBytes(this ulong? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }

        public static byte[] GetBytes(this double? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }

        public static byte[] GetBytes(this bool? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }


        /// <summary>
        /// Write UInt64 Epoch-encoded time stamp.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this DateTime time) {
            return time.ToUnixTimeAsUInt64().GetBytes();
        }

        /// <summary>
        /// Write UInt64 Epoch-encoded time stamp.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this DateTime? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }

        public static byte[] GetBytes(this string target) {
            if (null == target) {
                return ((UInt16)0).GetBytes();
            }

            var byteArray = Encoding.UTF8.GetBytes(target);
            return ((UInt16)byteArray.Length).GetBytes().Concat(byteArray).ToArray();
        }

        public static byte[] GetBytes(this Guid? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }

        public static byte[] GetBytes(this Guid v) {
            return v.ToByteArray();
        }

        public static byte[] GetBytes(this TimeSpan target) {
            return ((uint)target.TotalSeconds).GetBytes();
        }

        public static byte[] GetBytes(this TimeSpan? v) {
            return v.HasValue ? NullableWrapper(v.Value.GetBytes()) : NullableWrapper(null);
        }

        /// <summary>
        /// Prefixes the proper boolean depending on if the byte[] has data or not.
        /// </summary>
        /// <param name="payload"></param>
        private static byte[] NullableWrapper(byte[] payload) {
            if (payload == null || payload.Length == 0) {
                return false.GetBytes();
            }
            return true.GetBytes().Concat(payload).ToArray();
        }
    }
}
