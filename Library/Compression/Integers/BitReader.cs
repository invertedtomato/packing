using System;
using System.IO;

namespace InvertedTomato.Compression.Integers;

public class BitReader : IDisposable
{
    private readonly Stream Underlying;
    private readonly Boolean OwnUnderlying;

    public Boolean IsDisposed { get; private set; }

    public BitReader(Stream underlying)
    {
        Underlying = underlying;
        OwnUnderlying = false;
    }

    public BitReader(Stream underlying, Boolean ownUnderlying)
    {
        Underlying = underlying;
        OwnUnderlying = ownUnderlying;
    }

    public SByte ReadInt8(ICodec codec)
    {
        throw new NotImplementedException();
    }

    public Int16 ReadInt16(ICodec codec)
    {
        throw new NotImplementedException();
    }

    public Int32 ReadInt32(ICodec codec)
    {
        throw new NotImplementedException();
    }

    public Int64 ReadInt64(ICodec codec)
    {
        throw new NotImplementedException();
    }

    public Byte ReadUInt8(ICodec codec)
    {
        throw new NotImplementedException();
    }

    public UInt16 ReadUInt16(ICodec codec)
    {
        throw new NotImplementedException();
    }

    public UInt32 ReadUInt32(ICodec codec)
    {
        throw new NotImplementedException();
    }

    public UInt64 ReadUInt64(ICodec codec)
    {
        throw new NotImplementedException();
    }

    public void AlignByte()
    {
        throw new NotImplementedException();
    }


    public void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;

        // TODO: Flush partial byte

        if (OwnUnderlying)
        {
            Underlying.Dispose();
        }
    }
}