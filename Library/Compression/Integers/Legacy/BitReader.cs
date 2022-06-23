using System;
using System.IO;

namespace InvertedTomato.Compression.Integers.Legacy {
	public class BitReader : IDisposable {
        /// <summary>
        ///     Underlying input stream.
        /// </summary>
        private readonly IByteReader Input;

        /// <summary>
        ///     Position within the currently buffered byte. Defaulted to the end of the buffer to force a new byte to be read on
        ///     the next request.
        /// </summary>
        private Int32 BufferPosition = 8;

        /// <summary>
        ///     Currently buffered byte being worked on.
        /// </summary>
        private Byte BufferValue;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="input"></param>
        public BitReader(IByteReader input) {
			if (null == input) {
				throw new ArgumentNullException(nameof(input));
			}

			Input = input;
		}

        /// <summary>
        ///     If the reader is disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

		public void Dispose() {
			Dispose(true);
		}

        /// <summary>
        ///     Read a set of bits. This uses ulong as a 64-bit buffer (don't think of it like an integer, think of it as a bit
        ///     buffer).
        /// </summary>
        /// <param name="count">Number of bits to read, starting from the least-significant-bit (right side).</param>
        /// <returns></returns>
        public UInt64 Read(Int32 count) {
			if (count > 64) {
				throw new ArgumentOutOfRangeException("Count must be between 0 and 64, not " + count + ".", nameof(count));
			}

			if (IsDisposed) {
				throw new ObjectDisposedException("this");
			}

			UInt64 output = 0;

			// While there is still bits being read
			while (count > 0) {
				// If needed, load byte
				PrepareBuffer();

				// Calculate number of bits to read in this cycle - either the number of bits being requested, or the number of bits left in the buffer, whichever is less
				var chunkSize = Math.Min(count, 8 - BufferPosition);

				// Make room in output for this number of bits
				output <<= chunkSize;

				// Add bit to output
				var mask = Byte.MaxValue;
				mask <<= 8 - chunkSize;
				mask >>= BufferPosition;
				output |= (UInt64) (BufferValue & mask) >> (8 - chunkSize - BufferPosition);

				// Reduce number of bits remaining to be written
				count -= chunkSize;

				// Increment position in buffer by the number of bits just retrieved
				BufferPosition += chunkSize;
			}

			return output;
		}

        /// <summary>
        ///     View the next bit without moving the underlying pointer.
        /// </summary>
        /// <returns></returns>
        public Boolean PeakBit() {
			if (IsDisposed) {
				throw new ObjectDisposedException("this");
			}

			// If needed, load byte
			PrepareBuffer();

			// Return bit from buffer
			return (BufferValue & (1 << (7 - BufferPosition))) > 0;
		}

		private void PrepareBuffer() {
			// If there are still unused bits in the buffer, do nothing
			if (BufferPosition < 8) {
				return;
			}

#if DEBUG
			// Throw exception on insane buffer position
			if (BufferPosition > 8) {
				throw new Exception("Invalid position " + BufferPosition + ". Position has been offset by an incorrect value.");
			}
#endif

			// Read next byte into buffer
			var buffer = Input.ReadByte();
			if (buffer < 0) {
				throw new EndOfStreamException();
			}

			BufferValue = (Byte) buffer;

			// Reset buffer position to the start
			BufferPosition = 0;
		}

		protected virtual void Dispose(Boolean disposing) {
			if (IsDisposed) {
				return;
			}

			IsDisposed = true;

			if (disposing) {
				// Dispose managed state (managed objects).
			}
		}
	}
}