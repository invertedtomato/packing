using System;
using System.IO;

namespace InvertedTomato.Feather {
    public abstract partial class ConnectionBase {
        /// <summary>
        /// Send a binary payload to the remote endpoint.
        /// </summary>
        /// <param name="opcode">The opcode to send down</param>
        /// <param name="payloads"></param>
        protected void Send(byte opcode, params byte[] parameters) { // This is here to make error codes easier to send.
            if (null == parameters) {
                throw new ArgumentNullException("parameters");
            }

            Send(opcode, parameters);
        }

        /// <summary>
        /// Send a binary payload to the remote endpoint.
        /// </summary>
        /// <param name="opcode">The opcode to send down</param>
        /// <param name="parameters"></param>
        protected void Send(byte opcode, params byte[][] parameters) {
            if (null == parameters) {
                throw new ArgumentNullException("parameters");
            }

            using (var stream = new MemoryStream()) {
                stream.Write(opcode);
                foreach (var parameter in parameters) {
                    stream.Write(parameter);
                }

                Send(stream.ToArray());
            }
        }
    }
}
