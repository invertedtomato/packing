using System;
using System.IO;

namespace InvertedTomato.Feather {
    internal static class Core {
        internal static byte[] PayloadsToBuffer(IPayload[] payloads) {
            // Fetch raw payloads while calculating buffer length
            var bufferLength = 0;
            var rawPayloads = new byte[payloads.Length][];
            for (var i = 0; i < payloads.Length; i++) {
                var payload = payloads[i];

                // Check if null
                if (null == payload) {
                    throw new ArgumentException("Contains null element.", "payloads");
                }

                // Get raw payload
                rawPayloads[i] = payload.ToByteArray();
                if (rawPayloads[i].Length > ushort.MaxValue) {
                    throw new InternalBufferOverflowException("Payload longer than 65KB.");
                }

                // Increment raw buffer length required
                bufferLength += 2 + rawPayloads[i].Length;
            }

            // Merge everything to be sent into a buffer
            var buffer = new byte[bufferLength];
            var pos = 0;
            foreach (var rawPayload in rawPayloads) {
                var rawPayloadRawLength = BitConverter.GetBytes((ushort)rawPayload.Length);
                Buffer.BlockCopy(rawPayloadRawLength, 0, buffer, pos, rawPayloadRawLength.Length); // Length
                Buffer.BlockCopy(rawPayload, 0, buffer, pos + rawPayloadRawLength.Length, rawPayload.Length); // Payload		 
                pos += rawPayloadRawLength.Length + rawPayload.Length;
            }

            return buffer;
        }
    }
}
