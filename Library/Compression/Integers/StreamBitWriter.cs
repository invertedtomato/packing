using System;
using System.IO;

namespace InvertedTomato.Compression.Integers;

public class StreamBitWriter : IBitWriter, IDisposable
{
    private readonly Stream Underlying;
    private readonly Boolean OwnUnderlying;
    
    public Boolean IsDisposed { get; private set; }
    
    public StreamBitWriter(Stream underlying)
    {
        Underlying = underlying;
        OwnUnderlying = false;
    }
    public StreamBitWriter(Stream underlying, Boolean ownUnderlying)
    {
        Underlying = underlying;
        OwnUnderlying = ownUnderlying;
    }

    public void WriteBit(Boolean bit)
    {
        throw new NotImplementedException();
    }
    
    public void WriteBits(UInt64 bits, int count)
    {
        throw new NotImplementedException();
    }

    public void WriteByte(Byte b)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Implicitly causes AlignBytes</remarks>
    public void Align()
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