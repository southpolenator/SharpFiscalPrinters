using System;
using System.Runtime.CompilerServices;

namespace SharpFiscalPrinters.FP550
{
    public unsafe struct Status
    {
        public const int BytesCount = 6;
        private fixed byte bytes[BytesCount];

        public Status(ReadOnlySpan<byte> buffer)
        {
            if (buffer.Length < BytesCount)
                throw new ArgumentOutOfRangeException(nameof(buffer), $"Buffer needs to be at least {BytesCount} bytes long.");
            for (int i = 0; i < BytesCount; i++)
                bytes[i] = buffer[i];
        }

        public bool HasError => HasGeneralError || HasGeneralError1 || HasGeneralError2;

        public bool HasGeneralError => IsDateError || IsBiggerAmount || IsModuleMissingError || IsControlRibbonError;

        public bool HasGeneralError1 => CheckBit(0, 5) || IsCodeError || IsMechanismError || IsSyntaxError || IsRamError || IsMemoryCleared || IsNotAllowedError || IsOutOfRibbonError;

        public bool HasGeneralError2 => CheckBit(4, 5) || IsOutOfMemoryError || IsWriteError || IsReadOnlyError || IsDailyReportError;

        public bool HasWarning => IsDisplayDisconnected || IsCoverOpened || IsControlRibbonLow || IsRibbonLow || IsSpaceLow;

        public bool IsCodeError => CheckBit(0, 1);

        public bool IsMechanismError => CheckBit(0, 4);

        public bool IsSyntaxError => CheckBit(0, 0);

        public bool IsRamError => CheckBit(1, 4);

        public bool IsMemoryCleared => CheckBit(1, 2);

        public bool IsNotAllowedError => CheckBit(1, 1);

        public bool IsOutOfRibbonError => CheckBit(2, 0);

        public bool IsOutOfMemoryError => CheckBit(4, 4);

        public bool IsWriteError => CheckBit(4, 0);

        public bool IsReadOnlyError => CheckBit(5, 0);

        public bool IsDailyReportError => CheckBit(5, 2);

        public bool IsDateError => CheckBit(0, 2);

        public bool IsBiggerAmount => CheckBit(1, 0);

        public bool IsModuleMissingError => CheckBit(4, 2);

        public bool IsControlRibbonError => CheckBit(2, 2);

        public bool IsDisplayDisconnected => CheckBit(0, 3);

        public bool IsCoverOpened => CheckBit(1, 5);

        public bool IsControlRibbonLow => CheckBit(2, 4);

        public bool IsRibbonLow => CheckBit(2, 1);

        public bool IsSpaceLow => CheckBit(4, 3);

        public bool IsFiscalized => CheckBit(5, 3);

        public bool IsFiscalReceiptOpened => CheckBit(2, 3);

        public bool IsNonFiscalReceiptOpened => CheckBit(2, 5);

        public void WriteToBuffer(Span<byte> buffer)
        {
            if (buffer.Length < BytesCount)
                throw new ArgumentOutOfRangeException(nameof(buffer), $"Buffer needs to be at least {BytesCount} bytes long.");
            for (int i = 0; i < BytesCount; i++)
                buffer[i] = bytes[i];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckBit(int index, int bit)
        {
            return (bytes[index] & (1 << bit)) != 0;
        }
    }
}
