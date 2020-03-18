using System;

namespace SharpFiscalPrinters.FP550
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    internal class ProgramFormatAttribute : Attribute
    {
        public ProgramFormatAttribute(string format)
        {
            Format = format;
        }

        public string Format { get; set; }
    }
}
