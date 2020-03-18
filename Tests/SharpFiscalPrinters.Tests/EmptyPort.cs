using System;

namespace SharpFiscalPrinters.Tests
{
    internal class EmptyPort : IPort
    {
        public static readonly EmptyPort Instance = new EmptyPort();

        public void Dispose()
        {
        }

        public byte ReadByte()
        {
            throw new NotImplementedException();
        }

        public void WriteBytes(ArraySegment<byte> bytes)
        {
            throw new NotImplementedException();
        }
    }
}
