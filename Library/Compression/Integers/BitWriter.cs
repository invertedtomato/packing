using System;
using System.IO;

namespace InvertedTomato.Compression.Integers;

public class BitWriter : IDisposable
{
    private readonly Stream Underlying;
    private readonly Boolean OwnUnderlying;
    
    public Boolean IsDisposed { get; private set; }
    
    public BitWriter(Stream underlying)
    {
        Underlying = underlying;
        OwnUnderlying = false;
    }
    public BitWriter(Stream underlying, Boolean ownUnderlying)
    {
        Underlying = underlying;
        OwnUnderlying = ownUnderlying;
    }
    
    public void WriteBit(Boolean value, ICodec codec)
    {
        throw new NotImplementedException();
    }
    public void WriteUInt8(Byte value, ICodec codec)
    {
        throw new NotImplementedException();
    }
    public void WriteUInt16(UInt16 value, ICodec codec)
    {
        throw new NotImplementedException();
    }
    public void WriteUInt32(UInt32 value, ICodec codec)
    {
        throw new NotImplementedException();
    }
    public void WriteUInt64(UInt64 value, ICodec codec)
    {
        throw new NotImplementedException();
    }
    public void WriteInt8(SByte value, ICodec codec)
    {
        throw new NotImplementedException();
    }
    public void WriteInt16(Int16 value, ICodec codec)
    {
        throw new NotImplementedException();
    }
    public void WriteInt32(Int32 value, ICodec codec)
    {
        throw new NotImplementedException();
    }
    public void WriteInt64(Int64 value, ICodec codec)
    {
        throw new NotImplementedException();
    }
    

    public void AlignByte()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Implicitly causes AlignBytes</remarks>
    public void Flush()
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