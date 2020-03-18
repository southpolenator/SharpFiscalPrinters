using System;

namespace SharpFiscalPrinters.FP550
{
    internal struct ReceivedMessage
    {
        public ArraySegment<byte> Bytes;
        public ArraySegment<byte> Data;
        public byte MessageId;
        public byte MessageNumber;
        public Status? PrinterStatus;
    }

    internal struct ReceivedPrinterMessage
    {
        public ArraySegment<byte> Bytes;
        public ArraySegment<byte> Data;
        public byte MessageId;
        public byte MessageNumber;
        public Status PrinterStatus;

        public static implicit operator ReceivedPrinterMessage(ReceivedMessage message)
        {
            if (message.PrinterStatus == null)
                throw new Exception("Not a printer message");
            return new ReceivedPrinterMessage()
            {
                Bytes = message.Bytes,
                Data = message.Data,
                MessageId = message.MessageId,
                MessageNumber = message.MessageNumber,
                PrinterStatus = message.PrinterStatus.Value,
            };
        }
    }

    internal struct ReceivedProgramMessage
    {
        public ArraySegment<byte> Bytes;
        public ArraySegment<byte> Data;
        public byte MessageId;
        public byte MessageNumber;

        public static implicit operator ReceivedProgramMessage(ReceivedMessage message)
        {
            if (message.PrinterStatus != null)
                throw new Exception("Not a program message");
            return new ReceivedProgramMessage()
            {
                Bytes = message.Bytes,
                Data = message.Data,
                MessageId = message.MessageId,
                MessageNumber = message.MessageNumber,
            };
        }
    }
}
