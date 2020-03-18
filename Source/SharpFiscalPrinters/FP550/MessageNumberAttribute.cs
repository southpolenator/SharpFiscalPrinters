using System;

namespace SharpFiscalPrinters.FP550
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    internal class MessageNumberAttribute : Attribute
    {
        public MessageNumberAttribute(byte messageNumber)
        {
            MessageNumber = messageNumber;
        }

        public byte MessageNumber { get; set; }
    }
}
