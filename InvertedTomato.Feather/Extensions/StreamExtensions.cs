using InvertedTomato;
using System;
using System.IO;
using System.Text;

namespace InvertedTomato.Feather.Extensions {
    public static class StreamExtensions {
        #region DateTime
        /// <summary>
        /// Read UInt64 Epoch-encoded time stamp.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static DateTime ReadDateTime(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            // Read value
            var raw = target.ReadUInt64();

            // Return value
            return DateUtility.FromUnixTimestamp(raw);
        }

        /// <summary>
        /// Write UInt64 Epoch-encoded time stamp.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Stream Write(this Stream target, DateTime time) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            target.Write(time.ToUnixTimeAsUInt64());
            return target;
        }


        /// <summary>
        /// Read UInt64 Epoch-encoded time stamp as null-able.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static DateTime? ReadNullableDateTime(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            // Read value
            var raw = target.ReadUInt64();

            // If null, return null
            if (0 == raw) {
                return null;
            }

            // Return value
            return DateUtility.FromUnixTimestamp(raw);
        }


        /// <summary>
        /// Write UInt64 Epoch-encoded time stamp as null-able.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Stream Write(this Stream target, DateTime? time) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            if (!time.HasValue) {
                target.Write((ulong)0);
            } else {
                target.Write(time.Value.ToUnixTimeAsUInt64());
            }
            return target;
        }
        #endregion

        #region String
        /// <summary>
        /// Reads a string from the memory stream
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static string ReadString(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            // Read length value
            var length = target.ReadUInt16();

            var decoder = Encoding.UTF8.GetDecoder();
            StreamReader reader = new StreamReader(target, Encoding.UTF8);
            // Create buffer to hold data
            byte[] buffer = new byte[length];
            target.Read(buffer, 0, length);
            // Convert string
            return Encoding.UTF8.GetString(buffer);
        }


        public static Stream Write(this Stream target, string value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            var byteArray = Encoding.UTF8.GetBytes(value);
            target.Write((UInt16)byteArray.Length);
            target.Write(byteArray);
            return target;
        }
        #endregion

        #region Guid

        /// <summary>
        /// Reads a Guid from the memory stream
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Guid ReadGuid(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            // Read value
            return new Guid(target.Read(16));
        }

        /// <summary>
        /// Reads the nullable unique identifier.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">target</exception>
        public static Guid? ReadNullableGuid(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            // Read first byte to check whether the value is nullable.
            var hasValue = Convert.ToBoolean(target.ReadUInt8());
            if (hasValue) {
                return new Guid(target.Read(16));
            }
            return null;
        }

        /// <summary>
        /// Writes a Guid to the stream
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Stream Write(this Stream target, Guid value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            target.Write(value.ToByteArray());
            return target;
        }
        #endregion

        #region Boolean
        /// <summary>
        /// Reads a Boolean from the memory stream
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool ReadBoolean(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            // Read value
            var value = target.ReadByte();
            if (value > 1) {
                throw new ArgumentOutOfRangeException("Unable to parse boolean");
            }
            return value == 1;
        }

        /// <summary>
        /// Writes a Boolean to the stream
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Stream Write(this Stream target, Boolean value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            target.Write((byte)(value ? 1 : 0));
            return target;
        }
		#endregion
		
		#region TimeSpan
		/// <summary>
		/// Reads a TimeSpan from the memory stream
		/// </summary>
		/// <param name="target"></param>
		/// <returns></returns>
		public static TimeSpan ReadTimeSpan(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            // Read value
            var value = target.ReadUInt32();
            return TimeSpan.FromSeconds(value);
        }

        /// <summary>
        /// Writes a TimeSpan to the stream
        /// </summary>
        /// <param name="target"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Stream Write(this Stream target, TimeSpan value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            target.Write((Int32)value.TotalSeconds);
            return target;
        }
        #endregion
    }
}
