using System;

namespace SharpFiscalPrinters.FP550.Exceptions
{
    public class PrinterStatusException : Exception
    {
        public PrinterStatusException(Status printerStatus)
        {
            PrinterStatus = printerStatus;
        }

        public Status PrinterStatus { get; private set; }
    }
}
