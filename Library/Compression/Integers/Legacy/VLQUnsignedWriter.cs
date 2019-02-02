using System;
using System.IO;
using InvertedTomato.IO.Bits;

namespace InvertedTomato.Compression.Integers {
    /// <summary>
    ///     Writer for Variable-length Quantity (VLQ) unsigned numbers.
    /// </summary>
    [Obsolete("Consider using VLQCodec instead. It's faster and easier.")]
	public class VLQUnsignedWriter : IUnsignedWriter {
        /// <summary>
        ///     The stream to output encoded bytes to.
        /// </summary>
        private readonly BitWriter Output;

        /// <summary>
        ///     Number of bits to include in each packet.
        /// </summary>
        private readonly Int32 PacketSize;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public VLQUnsignedWriter(Stream output) : this(output, 7) { }

        /// <summary>
        ///     Instantiate with options
        /// </summary>
        /// <param name="output"></param>
        /// <param name="packetSize">The number of bits to include in each packet.</param>
        public VLQUnsignedWriter(Stream output, Int32 packetSize) {
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			// Store
			Output = new BitWriter(output);
			PacketSize = packetSize;
		}

        /// <summary>
        ///     If disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        ///     Append value to stream.
        /// </summary>
        /// <param name="value"></param>
        public void Write(UInt64 value) {
			if (IsDisposed) {
				throw new ObjectDisposedException("this");
			}

			// Calculate size of non-final packet
			var min = UInt64.MaxValue >> (64 - PacketSize);

			// Iterate through input, taking X bits of data each time, aborting when less than X bits left
			while (value > min) {
				// Write continuity header - more packets following
				Output.Write(0, 1);

				// Write payload
				Output.Write(value, PacketSize);

				// Offset value for next cycle
				value >>= PacketSize;
				value--;
			}

			// Write continuity header - no packets following
			Output.Write(1, 1);

			// Write final payload
			Output.Write(value, PacketSize);
		}

        /// <summary>
        ///     Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        public void Dispose() {
			Dispose(true);
		}

        /// <summary>
        ///     Write a given value.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static Byte[] WriteOneDefault(UInt64 value) {
			using (var stream = new MemoryStream()) {
				using (var writer = new VLQUnsignedWriter(stream)) {
					writer.Write(value);
				}

				return stream.ToArray();
			}
		}

        /// <summary>
        ///     Calculate the length of an encoded value in bits.
        /// </summary>
        /// <param name="packetSize">The number of bits to include in each packet.</param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32 CalculateBitLength(Int32 packetSize, UInt64 value) {
			var packets = (Int32) Math.Ceiling(BitOperation.CountUsed(value) / (Single) packetSize);

			return packets * (packetSize + 1);
		}

        /// <summary>
        ///     Flush any unwritten bits and dispose.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(Boolean disposing) {
			if (IsDisposed) {
				return;
			}

			IsDisposed = true;

			if (disposing) {
				// Dispose managed state (managed objects)
				Output?.Dispose();
			}
		}
	}
}