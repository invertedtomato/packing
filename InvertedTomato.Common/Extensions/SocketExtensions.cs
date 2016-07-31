using System;
using System.IO;
using System.Net.Sockets;

namespace InvertedTomato {
    public static class SocketExtensions {
        /// <summary>
        /// Read the next received byte.
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static byte ReadByte(this Socket target) {
            if (null == target) {
                throw new ArgumentNullException("target");
            }

            var buffer = new byte[1];
            if (target.Receive(buffer) != buffer.Length) {
                throw new EndOfStreamException();
            }

            return buffer[0];
        }


        /// <summary>
        /// Setup TCP keep-alive
        /// </summary>
        /// <param name="enabled">If TCP keep-alives are to be enabled</param>
        public static void SetKeepAlive(this Socket target, bool enabled) { target.SetKeepAlive(enabled, 2 * 60 * 60 * 1000); }

        /// <summary>
        /// Setup TCP keep-alive
        /// </summary>
        /// <param name="enabled">If TCP keep-alives are to be enabled</param>
        /// <param name="idle">Amount of time socket needs to be idle before keep-alive is sent (milliseconds).</param>
        public static void SetKeepAlive(this Socket target, bool enabled, ulong idle) { target.SetKeepAlive(enabled, idle, 1 * 1000); }

        /// <summary>
        /// Setup TCP keep-alive
        /// </summary>
        /// <param name="enabled">If TCP keep-alives are to be enabled</param>
        /// <param name="idle">Amount of time socket needs to be idle before keep-alive is sent (milliseconds).</param>
        /// <param name="interval">The delay between keep-alive retries (milliseconds).</param>
        public static void SetKeepAlive(this Socket target, bool enabled, ulong idle, ulong interval) {
            const int bytesPerLong = 4; // 32 / 8
            const int bitsPerByte = 8;

            // Prepare inputs
            var input = new ulong[3];
            input[0] = enabled ? 0UL : 1UL;
            input[1] = idle;
            input[2] = interval;

            // Pack input into byte struct
            var SIO_KEEPALIVE_VALS = new byte[3 * bytesPerLong];
            for (byte i = 0; i < input.Length; i++) {
                SIO_KEEPALIVE_VALS[i * bytesPerLong + 3] = (byte)(input[i] >> ((bytesPerLong - 1) * bitsPerByte) & 0xff);
                SIO_KEEPALIVE_VALS[i * bytesPerLong + 2] = (byte)(input[i] >> ((bytesPerLong - 2) * bitsPerByte) & 0xff);
                SIO_KEEPALIVE_VALS[i * bytesPerLong + 1] = (byte)(input[i] >> ((bytesPerLong - 3) * bitsPerByte) & 0xff);
                SIO_KEEPALIVE_VALS[i * bytesPerLong + 0] = (byte)(input[i] >> ((bytesPerLong - 4) * bitsPerByte) & 0xff);
            }

            // Create bytestruct for result (bytes pending on server socket)
            var result = new byte[4];

            // Write SIO_VALS to Socket IOControl
            target.IOControl(IOControlCode.KeepAliveValues, SIO_KEEPALIVE_VALS, result);
        }
    }
}
