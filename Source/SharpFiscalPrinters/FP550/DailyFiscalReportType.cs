namespace SharpFiscalPrinters.FP550
{
    public enum DailyFiscalReportType
    {
        /// <summary>
        /// Prints daily report with clearing data.
        /// </summary>
        DailyReport = 0,

        /// <summary>
        /// Prints report without clearing data.
        /// </summary>
        Overview = 1,

        /// <summary>
        /// Prints report without clearing data with additional info.
        /// </summary>
        OverviewExtended = 2,
    }
}
