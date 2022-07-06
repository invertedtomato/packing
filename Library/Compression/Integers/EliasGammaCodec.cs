using System;

namespace InvertedTomato.Compression.Integers
{
    public class EliasGammaCodec : ICodec
    {
        public UInt64 MinValue => UInt64.MinValue;
        public UInt64 MaxValue => UInt64.MaxValue - 1; // TODO: Check!

        private void Encode(UInt64 value, IBitWriter buffer)
        {
            // Offset value to allow zeros
            value++;

            // Calculate length
            var length = BitOperation.CountUsed(value);

            // Write unary zeros
            buffer.WriteBits(0, length - 1);

            // Write value
            buffer.WriteBits(value, length);
        }

        private UInt64 Decode(IBitReader buffer)
        {
            // Read length
            var length = 1;
            while (!buffer.PeakBit())
            {
                // Note that length is one bit longer
                length++;

                // Remove 0 from input
                buffer.ReadBit();
            }

            // Read value
            var value = buffer.ReadBits(length);

            // Remove offset from value
            value--;

            return value;
        }

        public Int32? CalculateEncodedBits(UInt64 value)
        {
            // Offset for zero
            value++;

            return BitOperation.CountUsed(value) * 2 - 1;
        }

        public void EncodeBit(bool value, IBitWriter buffer) => Encode(1, buffer);
        public void EncodeUInt8(byte value, IBitWriter buffer) => Encode(value, buffer);
        public void EncodeUInt16(ushort value, IBitWriter buffer) => Encode(value, buffer);
        public void EncodeUInt32(uint value, IBitWriter buffer) => Encode(value, buffer);
        public void EncodeUInt64(ulong value, IBitWriter buffer) => Encode(value, buffer);
        public void EncodeInt8(sbyte value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);
        public void EncodeInt16(short value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);
        public void EncodeInt32(int value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);
        public void EncodeInt64(long value, IBitWriter buffer) => Encode(ZigZag.Encode(value), buffer);

        public Boolean DecodeBit(IBitReader buffer) => Decode(buffer) > 0;
        public Byte DecodeUInt8(IBitReader buffer) => (Byte) Decode(buffer);
        public UInt16 DecodeUInt16(IBitReader buffer) => (UInt16) Decode(buffer);
        public UInt32 DecodeUInt32(IBitReader buffer) => (UInt32) Decode(buffer);
        public UInt64 DecodeUInt64(IBitReader buffer) => Decode(buffer);
        public SByte DecodeInt8(IBitReader buffer) => (SByte) ZigZag.Decode(Decode(buffer));
        public Int16 DecodeInt16(IBitReader buffer) => (Int16) ZigZag.Decode(Decode(buffer));
        public Int32 DecodeInt32(IBitReader buffer) => (Int32) ZigZag.Decode(Decode(buffer));
        public Int64 DecodeInt64(IBitReader buffer) => ZigZag.Decode(Decode(buffer));
    }
}