using System;
using System.IO;
using System.Text;

namespace InvertedTomato {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public static class StreamExtensions {
        /// <summary>
        /// Read all contents to byte array.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static byte[] ReadRemaining(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            if (target is MemoryStream) { // Don't bother creating a new memory stream.
                return ((MemoryStream)target).ToArray();
            }
            using (var memoryStream = new MemoryStream()) {
                target.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Rewind the stream to it's beginning.
        /// </summary>
        /// <param name="target"></param>
        public static void Rewind(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            target.Seek(0, SeekOrigin.Begin);
        }

        public static bool TryRead(this Stream target, out sbyte value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[1];
            if (target.Read(buffer, 0, 1) != 1) {
                value = default(sbyte);
                return false;
            }

            value = (sbyte)(buffer[0] - sbyte.MaxValue); // TODO: Check this line is correct
            return true;
        }

        public static bool TryRead(this Stream target, out byte value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[1];
            if (target.Read(buffer, 0, 1) != 1) {
                value = default(byte);
                return false;
            }

            value = buffer[0];
            return true;
        }

        public static bool TryRead(this Stream target, out short value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[2];
            if (target.Read(buffer, 0, 2) != 2) {
                value = default(short);
                return false;
            }

            value = BitConverter.ToInt16(buffer, 0);
            return true;
        }

        public static bool TryRead(this Stream target, out ushort value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[2];
            if (target.Read(buffer, 0, 2) != 2) {
                value = default(ushort);
                return false;
            }

            value = BitConverter.ToUInt16(buffer, 0);
            return true;
        }

        public static bool TryRead(this Stream target, out int value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[4];
            if (target.Read(buffer, 0, 4) != 4) {
                value = default(int);
                return false;
            }

            value = BitConverter.ToInt32(buffer, 0);
            return true;
        }

        public static bool TryRead(this Stream target, out uint value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[4];
            if (target.Read(buffer, 0, 4) != 4) {
                value = default(uint);
                return false;
            }

            value = BitConverter.ToUInt32(buffer, 0);
            return true;
        }

        public static bool TryRead(this Stream target, out long value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[8];
            if (target.Read(buffer, 0, 8) != 8) {
                value = default(long);
                return false;
            }

            value = BitConverter.ToInt64(buffer, 0);
            return true;
        }

        public static bool TryRead(this Stream target, out ulong value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[8];
            if (target.Read(buffer, 0, 8) != 8) {
                value = default(ulong);
                return false;
            }

            value = BitConverter.ToUInt64(buffer, 0);
            return true;
        }

        public static bool TryRead(this Stream target, byte[] buffer) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            if (null == buffer) {
                throw new ArgumentNullException("buffer");
            }

            if (target.Read(buffer, 0, buffer.Length) != buffer.Length) {
                return false;
            }

            return true;
        }

        public static int SwapEndianness(int value) {
            var b1 = (value >> 0) & 0xff;
            var b2 = (value >> 8) & 0xff;
            var b3 = (value >> 16) & 0xff;
            var b4 = (value >> 24) & 0xff;

            return b1 << 24 | b2 << 16 | b3 << 8 | b4 << 0;
        }

        public static bool TryReadBigEndian(this Stream target, out sbyte value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[1];
            if (target.Read(buffer, 0, 1) != 1) {
                value = default(sbyte);
                return false;
            }

            value = (sbyte)(buffer[0] - sbyte.MaxValue);
            return true;
        }

        public static bool TryReadBigEndian(this Stream target, out byte value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[1];
            if (target.Read(buffer, 0, 1) != 1) {
                value = default(byte);
                return false;
            }

            value = (byte)(buffer[0] - byte.MaxValue);
            return true;
        }

        public static bool TryReadBigEndian(this Stream target, out short value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[2];
            if (target.Read(buffer, 0, 2) != 2) {
                value = default(short);
                return false;
            }

            Array.Reverse(buffer);

            value = BitConverter.ToInt16(buffer, 0);
            return true;
        }

        public static bool TryReadBigEndian(this Stream target, out ushort value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[2];
            if (target.Read(buffer, 0, 2) != 2) {
                value = default(ushort);
                return false;
            }

            Array.Reverse(buffer);

            value = BitConverter.ToUInt16(buffer, 0);
            return true;
        }

        public static bool TryReadBigEndian(this Stream target, out int value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[4];
            if (target.Read(buffer, 0, 4) != 4) {
                value = default(int);
                return false;
            }

            Array.Reverse(buffer);

            value = BitConverter.ToInt32(buffer, 0);
            return true;
        }

        public static bool TryReadBigEndian(this Stream target, out uint value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[4];
            if (target.Read(buffer, 0, 4) != 4) {
                value = default(uint);
                return false;
            }

            Array.Reverse(buffer);

            value = BitConverter.ToUInt32(buffer, 0);
            return true;
        }

        public static bool TryReadBigEndian(this Stream target, out long value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[8];
            if (target.Read(buffer, 0, 8) != 8) {
                value = default(long);
                return false;
            }

            Array.Reverse(buffer);

            value = BitConverter.ToInt64(buffer, 0);
            return true;
        }

        public static bool TryReadBigEndian(this Stream target, out ulong value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[8];
            if (target.Read(buffer, 0, 8) != 8) {
                value = default(ulong);
                return false;
            }

            Array.Reverse(buffer);

            value = BitConverter.ToUInt64(buffer, 0);
            return true;
        }

        public static sbyte ReadInt8BigEndian(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            sbyte value;
            if (!target.TryReadBigEndian(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static byte ReadUInt8BigEndian(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            byte value;
            if (!target.TryReadBigEndian(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static short ReadInt16BigEndian(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            short value;
            if (!target.TryReadBigEndian(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static ushort ReadUInt16BigEndian(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            ushort value;
            if (!target.TryReadBigEndian(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static int ReadInt32BigEndian(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            int value;
            if (!target.TryReadBigEndian(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static uint ReadUInt32BigEndian(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            uint value;
            if (!target.TryReadBigEndian(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static long ReadInt64BigEndian(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            long value;
            if (!target.TryReadBigEndian(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static ulong ReadUInt64BigEndian(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            ulong value;
            if (!target.TryReadBigEndian(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static byte[] Read(this Stream target, int count) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[count];
            if (target.Read(buffer, 0, count) != count) {
                throw new EndOfStreamException();
            }

            return buffer;
        }

        public static sbyte ReadInt8(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            sbyte value;
            if (!target.TryRead(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static byte ReadUInt8(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            byte value;
            if (!target.TryRead(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static short ReadInt16(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            short value;
            if (!target.TryRead(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static ushort ReadUInt16(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            ushort value;
            if (!target.TryRead(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static int ReadInt32(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            int value;
            if (!target.TryRead(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static uint ReadUInt32(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            uint value;
            if (!target.TryRead(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static long ReadInt64(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            long value;
            if (!target.TryRead(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static ulong ReadUInt64(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            ulong value;
            if (!target.TryRead(out value)) {
                throw new EndOfStreamException();
            }

            return value;
        }

        public static double ReadDouble(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            using (var r = new BinaryReader(target)) {
                return r.ReadDouble();
            }
        }

        public static float ReadFloat(this Stream target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            using (var r = new BinaryReader(target)) {
                return r.ReadSingle();
            }
        }

        public static void Write(this Stream target, sbyte value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            target.Write(BitConverter.GetBytes(value), 0, 1);
        }

        public static void Write(this Stream target, byte value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            target.Write(BitConverter.GetBytes(value), 0, 1);
        }

        public static void Write(this Stream target, short value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            target.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public static void Write(this Stream target, ushort value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            target.Write(BitConverter.GetBytes(value), 0, 2);
        }

        public static void Write(this Stream target, int value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            target.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public static void Write(this Stream target, uint value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            target.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public static void Write(this Stream target, long value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            target.Write(BitConverter.GetBytes(value), 0, 8);
        }

        public static void Write(this Stream target, ulong value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            target.Write(BitConverter.GetBytes(value), 0, 8);
        }

        public static void Write(this Stream target, byte[] value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            if (null == value) {
                throw new ArgumentNullException("value");
            }

            target.Write(value, 0, value.Length);
        }

        public static void Write(this Stream target, double value) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }
            var data = BitConverter.GetBytes(value);
            target.Write(data, 0, data.Length);
        }







        /*
		/// <summary>
		/// Read all contents to string.
		/// </summary>
		/// <param name="v"></param>
		/// <returns></returns>
		[Obsolete("Does not take into account string encoding. Use ReadRemaining().ToStringDefaultEncoding() instead.")]
		public static string ReadAllToString(this Stream target) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}

			target.Rewind();
			return (new ASCIIEncoding()).GetString(target.ReadRemaining());
		}
		
		/// <summary>
		/// Read the first X KB of stream to string.
		/// </summary>
		/// <param name="v"></param>
		/// <param name="maxKB"></param>
		/// <returns></returns>
		[Obsolete("Does not take into account string encoding. Use ReadRemaining().ToStringDefaultEncoding() instead.")]
		public static string ReadToString(this Stream target, int maxKB) {
			if (null == target) {
				throw new ArgumentNullException("target");
			}
			if (long.MaxValue / 1024 < (long)maxKB) {
				throw new ArgumentException("Must not exceed " + long.MaxValue / 1024 + "b.", "maxKB");
			}

			// Prepare space
			var output = "";
			var buffer = new byte[1024];
			var remaining = target.Length;

			// Limit fetch size
			if (remaining > (long)maxKB * 1024)
				remaining = (long)maxKB * 1024;

			// Read
			while (remaining > 0) {
				var chunk = target.Read(buffer, 0, 1024);
				output += System.Text.Encoding.UTF8.GetString(buffer, 0, chunk);
				remaining -= chunk;
			}

			return output;
		}
		*/
    }
}
