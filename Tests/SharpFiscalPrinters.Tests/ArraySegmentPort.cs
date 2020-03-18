using System;
using System.Collections.Generic;

namespace SharpFiscalPrinters.Tests
{
    internal class ArraySegmentPort : IPort
    {
        private List<byte> writtenBytes = new List<byte>();

        public ArraySegmentPort(ArraySegment<byte> bytes)
        {
            Bytes = bytes;
            Position = 0;
        }

        public IReadOnlyList<byte> WrittenBytes => writtenBytes;

        public ArraySegment<byte> Bytes { get; private set; }

        public int Position { get; private set; }

        public void Dispose()
        {
        }

        public byte ReadByte()
        {
            return Bytes[Position++];
        }

        public void WriteBytes(ArraySegment<byte> bytes)
        {
            writtenBytes.AddRange(bytes);
        }
    }
}
