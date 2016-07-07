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
    }
}
