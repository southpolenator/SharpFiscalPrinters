using System;

namespace SharpFiscalPrinters.FP550
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    internal class PrinterFormatAttribute : Attribute
    {
        public PrinterFormatAttribute(string format)
        {
            Format = format;
        }

        public string Format { get; set; }
    }
}
