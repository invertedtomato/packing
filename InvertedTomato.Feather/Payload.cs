using System;
using System.Linq;

namespace InvertedTomato.Feather {
    public sealed class Payload {
        private byte[] payloadBuffer;

        public byte Opcode { get; }
        public byte[] Parameters { get; }
        public int Length { get { return 1 + Parameters.Length; } }

        public Payload(byte opcode, byte[] parameters) {
            if (null == parameters) {
                throw new ArgumentNullException("parameters");
            }

            Opcode = opcode;
            Parameters = parameters;
        }

        public Payload(byte opcode, params byte[][] parameters) {
            Opcode = opcode;

            // Merge everthing to be sent into a buffer
            Parameters = new byte[parameters.Sum(a => a.Length)];
            var pos = 0;
            foreach (var parameter in parameters) {
                Buffer.BlockCopy(parameter, 0, Parameters, pos, parameter.Length);
                pos += parameter.Length;
            }
        }

        public Payload(byte[] payload) {
            // NOTE: possible to optimise this into the low-level receive logic, but is it worth it?
            Opcode = payload[0];
            Parameters = new byte[payload.Length - 1];
            Buffer.BlockCopy(payload, 1, Parameters, 0, Parameters.Length);
        }
    }
}
