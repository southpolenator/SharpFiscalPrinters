using System;

namespace SharpFiscalPrinters
{
    public interface IPort : IDisposable
    {
        byte ReadByte();

        void WriteBytes(ArraySegment<byte> bytes);
    }
}
