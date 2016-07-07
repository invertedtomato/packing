using System;
using System.Linq;
using System.Net;

namespace InvertedTomato {
    public static class IPAddressUtility {
        /// <summary>
        /// Decode an IP address from a padded byte array.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        public static IPAddress FromBytes(byte[] raw) {
            if (null == raw) {
                throw new ArgumentNullException("raw");
            }

            // If IPv4 address hiding in a v6 address...
            if (raw.Length == 16 && // It's 16 bytes long
                raw.Take(12).All(a => a == 0) && // It starts with 12 0s
                !raw.Skip(12).All(a => a == 0)) { // It doesn't end with 4 0s
                // Extract v4 address
                var v4 = new byte[4];
                Buffer.BlockCopy(raw, 12, v4, 0, 4);

                // Return address
                return new IPAddress(v4);
            }

            return new IPAddress(raw);
        }
    }
}
