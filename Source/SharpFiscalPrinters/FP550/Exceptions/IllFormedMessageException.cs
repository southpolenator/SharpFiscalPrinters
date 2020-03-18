using System;

namespace SharpFiscalPrinters.FP550.Exceptions
{
    public class IllFormedMessageException : Exception
    {
        public IllFormedMessageException(string message)
            : base(message)
        {
        }
    }
}
