using SharpFiscalPrinters.FP550.Exceptions;
using System;
using System.Text;

namespace SharpFiscalPrinters.FP550
{
    public class ExtendedFP550 : FP550
    {
        public const string InvalidItemText = "Invalid item because of reset";
        private static string[] taxRatesMapping = new[] { "А", "Г", "Д", "Ђ", "Е", "Ж", "И", "Ј", "К" };

        public ExtendedFP550(IPort port, byte startingMessageId = Communication.MinimumMessageId, Encoding? communicationEncoding = null)
            : base(port, port, startingMessageId, communicationEncoding)
        {
        }

        public ExtendedFP550(IPort sendingPort, IPort receivingPort, byte startingMessageId = Communication.MinimumMessageId, Encoding? communicationEncoding = null)
            : base(sendingPort, receivingPort, startingMessageId, communicationEncoding)
        {
        }

        public event Action? OutOfRibbonNotification;

        public bool IsPrinterFiscalized()
        {
            return GetExtendedPrinterStatus().IsFiscalized;
        }

        public bool IsFiscalReceiptOpened()
        {
            return GetPrinterStatus().IsFiscalReceiptOpened;
        }

        public bool IsNonFiscalReceiptOpened()
        {
            return GetPrinterStatus().IsNonFiscalReceiptOpened;
        }

        public void OpenFiscalReceipt(string cachier, string password, int cashRegisterNumber, Func<bool> shouldCloseFiscalReceipt, Func<bool> shouldCloseNonFiscalReceipt)
        {
            CloseOpenedReceipt(shouldCloseFiscalReceipt, shouldCloseNonFiscalReceipt);
            OpenFiscalReceipt(cachier, password, cashRegisterNumber);
        }

        public double GetFiscalReceiptAmountToPay()
        {
            GetFiscalTransactionStatus(true, out var _, out var _, out var totalSum, out var paidSum);
            return totalSum - paidSum;
        }

        public bool IsFirstItemInvalid()
        {
            int itemId = 1;

            return GetItem(ref itemId, out var taxRate, out var price, out var quantity, out var name) && name == InvalidItemText;
        }

        public void DeleteFirstItem()
        {
            int count = GetNuberOfItems();
            int itemId = 1;
            bool hasFirst = GetItem(ref itemId, out string _, out double _, out double _, out string name);

            if (!hasFirst)
            {
                AddInvalidItem(1);
                return;
            }

            if (count == 1 && hasFirst)
            {
                if (name == InvalidItemText)
                    return;
                AddInvalidItem(2);
                DeleteItem(1);
                AddInvalidItem(1);
                DeleteItem(2);
                return;
            }

            DeleteItem(1);
            AddInvalidItem(1);
        }

        public void AddInvalidItem(int itemId = 1)
        {
            var taxRates = GetTaxRates(out int decimals);
            for (int i = 0; i < taxRates.Length; i++)
                if (taxRates[i] != null)
                {
                    string name = InvalidItemText;
                    string taxRate = taxRatesMapping[i];

                    if (itemId != 1)
                        name += itemId.ToString();
                    AddItem(taxRate, itemId, 1, name);
                }
            throw new Exception("Something went wrong. There should be at least one tax rate.");
        }

        public int GetNuberOfItems()
        {
            GetItemsInfo(out var _, out var _, out int numberOfItems);
            return numberOfItems;
        }

        public void OpenNonFiscalReceipt(Func<bool> shouldCloseFiscalReceipt, Func<bool> shouldCloseNonFiscalReceipt)
        {
            CloseOpenedReceipt(shouldCloseFiscalReceipt, shouldCloseNonFiscalReceipt);
            OpenNonFiscalReceipt();
        }

        public new void AddItem(string taxRate, int itemId, double price, string name)
        {
            base.AddItem(taxRate, itemId, price, name);
            if (itemId == 2 && IsFirstItemInvalid())
                DeleteItem(1);
        }

        protected override bool ShouldRetryMessage(Status printerStatus)
        {
            if (printerStatus.IsOutOfRibbonError || printerStatus.IsControlRibbonError)
            {
                FP550 testPrinter = new FP550(SendingPort, ReceivingPort);

                do
                {
                    // Notify user that printer is out of ribbon.
                    if (OutOfRibbonNotification != null)
                        OutOfRibbonNotification();
                    else
                    {
                        Console.WriteLine("Printer is out of ribbon.");
                        System.Threading.Thread.Sleep(1000);
                    }

                    try
                    {
                        testPrinter.GetItemsInfo(out int _, out int _, out int _);
                    }
                    catch (PrinterStatusException ex)
                    {
                        if (ex.PrinterStatus.IsOutOfRibbonError || ex.PrinterStatus.IsControlRibbonError)
                            continue;
                    }
                }
                while (false);
                return true;
            }
            return false;
        }

        private void CloseOpenedReceipt(Func<bool> shouldCloseFiscalReceipt, Func<bool> shouldCloseNonFiscalReceipt)
        {
            if (IsFiscalReceiptOpened())
            {
                if (shouldCloseFiscalReceipt())
                {
                    RecordPayment();
                    CloseFiscalReceipt();
                }
            }
            else if (IsNonFiscalReceiptOpened())
            {
                if (shouldCloseNonFiscalReceipt())
                    CloseNonFiscalReceipt();
            }
        }
    }
}
