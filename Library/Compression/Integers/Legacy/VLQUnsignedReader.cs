using System;
using System.IO;
using InvertedTomato.IO.Bits;

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    ///     Reader for Variable-length Quantity (VLQ) unsigned numbers.
    /// </summary>
    [Obsolete("Consider using VLQCodec instead. It's faster and easier.")]
	public class VLQUnsignedReader : IUnsignedReader {
        /// <summary>
        ///     The underlying stream to be reading from.
        /// </summary>
        private readonly BitReader Input;

        /// <summary>
        ///     Number of bits to include in each packet.
        /// </summary>
        private readonly Int32 PacketSize;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public VLQUnsignedReader(Stream input) : this(input, 7) { }

        /// <summary>
        ///     Instantiate with options.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="packetSize">The number of bits to include in each packet.</param>
        public VLQUnsignedReader(Stream input, Int32 packetSize) {
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			if (packetSize < 1 || packetSize > 32) {
				throw new ArgumentOutOfRangeException("PacketSize must be 1<=x<=32 not " + packetSize + ".", "packetSize");
			}

			// Store
			Input = new BitReader(input);
			PacketSize = packetSize;
		}

        /// <summary>
        ///     If disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        ///     Read the next value.
        /// </summary>
        /// <returns></returns>
        public UInt64 Read() {
			if (IsDisposed) {
				throw new ObjectDisposedException("this");
			}

			// Setup offset
			var outputPosition = 0;

			// Set value to 0
			UInt64 value = 0;

			Boolean final;
			do {
				// Read if this is the final packet
				final = Input.Read(1) > 0;

				// Read payload
				var chunk = Input.Read(PacketSize);

				// Add payload to value
				value += (chunk + 1) << outputPosition;

				// Update target offset
				outputPosition += PacketSize;
			} while (!final);

			// Remove zero offset
			value--;

			return value;
		}

        /// <summary>
        ///     Dispose.
        /// </summary>
        public void Dispose() {
			Dispose(true);
		}

        /// <summary>
        ///     Read first value from a byte array.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static UInt64 ReadOneDefault(Byte[] input) {
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			using (var stream = new MemoryStream(input)) {
				using (var reader = new VLQUnsignedReader(stream)) {
					return reader.Read();
				}
			}
		}

        /// <summary>
        ///     Dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(Boolean disposing) {
			if (IsDisposed) {
				return;
			}

			IsDisposed = true;

			if (disposing) {
				// Dispose managed state (managed objects)
				Input?.Dispose();
			}
		}
	}
}