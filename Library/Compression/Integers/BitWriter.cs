using System;
using System.IO;

namespace InvertedTomato.Compression.Integers {
	public class BitWriter : IDisposable {
        /// <summary>
        ///     Underlying stream to output to.
        /// </summary>
        private readonly IByteWriter Output;

        /// <summary>
        ///     The current bit within the current byte being worked on next.
        /// </summary>
        private Int32 BufferPosition;

        /// <summary>
        ///     The current byte being worked on.
        /// </summary>
        private Byte BufferValue;

        /// <summary>
        ///     Standard instantiation.
        /// </summary>
        /// <param name="output"></param>
        public BitWriter(IByteWriter output) {
			if (null == output) {
				throw new ArgumentNullException(nameof(output));
			}

			Output = output;
		}
		// LEAST significant BIT is on the RIGHT of the byte
		// LEAST significant BYTE is the FIRST in the stream
		// MOST significant BIT is on the LEFT of the byte
		// MOST significant BYTE is the LAST in the stream

        /// <summary>
        ///     If disposed.
        /// </summary>
        public Boolean IsDisposed { get; private set; }

		public void Dispose() {
			Dispose(true);
		}

        /// <summary>
        ///     Write a set of bits. This uses ulong as a 64-bit buffer (don't think of it like an integer, think of it as a bit
        ///     buffer).
        /// </summary>
        /// <param name="buffer">Buffer to write from.</param>
        /// <param name="count">Number of bits to write, starting from the least-significant-bit (right side).</param>
        public void Write(UInt64 buffer, Int32 count) {
			if (count > 64) {
				throw new ArgumentOutOfRangeException("Count must be between 0 and 64, not " + count + ".", nameof(count));
			}

			if (IsDisposed) {
				throw new ObjectDisposedException("this");
			}

			// Remove unwanted bits
			buffer <<= 64 - count;
			buffer >>= 64 - count;

			// While there are bits remaining
			while (count > 0) {
				// Calculate number of bits to write in this cycle - either the number of bits being requested, or the number of bits available in the buffer, whichever is less
				var chunkSize = Math.Min(count, 8 - BufferPosition);

				// Add bits to buffer
				if (BufferPosition + count > 8) {
					BufferValue |= (Byte) (buffer >> (count - chunkSize));
				} else {
					BufferValue |= (Byte) (buffer << (8 - BufferPosition - chunkSize));
				}

				// Reduce number of bits remaining to be written
				count -= chunkSize;

				// Increment position in the buffer
				BufferPosition += chunkSize;

				// If buffer is full...
				if (BufferPosition == 8) {
					// Flush buffer
					Flush();
				}

#if DEBUG
				// Catch insane situation
				if (BufferPosition > 8) {
					throw new Exception("Invalid position " + BufferPosition + ". Position has been offset by an incorrect value.");
				}
#endif
			}
		}

		private void Flush() {
			// Abort flush if buffer is empty
			if (BufferPosition == 0) {
				return;
			}

			// Flush buffer
			Output.WriteByte(BufferValue);

			// Clear buffer
			BufferValue = 0;

			// Reset buffer position
			BufferPosition = 0;
		}

		protected virtual void Dispose(Boolean disposing) {
			if (IsDisposed) {
				return;
			}

			IsDisposed = true;

			if (disposing) {
				// Flush buffer
				Flush();

				// Dispose managed state (managed objects)
			}
		}
	}
}