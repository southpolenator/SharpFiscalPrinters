using SharpFiscalPrinters.FP550.Exceptions;
using System;
using System.Text;

namespace SharpFiscalPrinters.FP550
{
    public class FP550 : IDisposable
    {
        private Communication programCommunication;
        private Communication printerCommunication;
        private byte nextMessageId;

        public FP550(IPort port, byte startingMessageId = Communication.MinimumMessageId, Encoding? communicationEncoding = null)
            : this(port, port, startingMessageId, communicationEncoding)
        {
        }

        public FP550(IPort sendingPort, IPort receivingPort, byte startingMessageId = Communication.MinimumMessageId, Encoding? communicationEncoding = null)
        {
            programCommunication = new Communication(sendingPort, communicationEncoding);
            printerCommunication = new Communication(receivingPort, communicationEncoding);
            nextMessageId = Math.Min(Math.Max(startingMessageId, Communication.MinimumMessageId), Communication.MaximumMessageId);
        }

        public IPort SendingPort => programCommunication.Port;

        public IPort ReceivingPort => printerCommunication.Port;

        private byte NextMessageId
        {
            get
            {
                var messageId = nextMessageId++;

                if (nextMessageId > Communication.MaximumMessageId)
                    nextMessageId = Communication.MinimumMessageId;
                return messageId;
            }
        }

        public void Dispose()
        {
            if (programCommunication.Port != printerCommunication.Port)
                printerCommunication.Port.Dispose();
            programCommunication.Port.Dispose();
        }

        public void OpenCashRegister()
        {
            SendAndReceive(MessageType.M106I, 23);
        }

        public Status GetExtendedPrinterStatus()
        {
            return SendAndReceive(MessageType.M74O, "X").PrinterStatus;
        }

        public Status GetPrinterStatus()
        {
            return SendAndReceive(MessageType.M74).PrinterStatus;
        }

        public string GetIdentificationNumberOfFiscalModule()
        {
            var message = SendAndReceive(MessageType.M90, "1");

            Parse(message, MessageType.M90, out string _, out string _, out string _, out string _, out string infm, out string _);
            return infm;
        }

        public string GetTaxIdentificationNumber()
        {
            var message = SendAndReceive(MessageType.M99);

            Parse(message, MessageType.M90, out string tin, out string _);
            return tin;
        }

        public void ChangeCashierPassword(string cachier, string oldPassword, string newPassword)
        {
            SendAndReceive(MessageType.M101, cachier, oldPassword, newPassword);
        }

        public DateTime GetDateTime()
        {
            var message = SendAndReceive(MessageType.M62);

            Parse(message, MessageType.M62, out int day, out int month, out int year, out int hour, out int minute, out int second);
            return new DateTime(year + 2000, month, day, hour, minute, second);
        }

        public void OpenFiscalReceipt(string cachier, string password, int cashRegisterNumber)
        {
            SendAndReceive(MessageType.M48, cachier, password, cashRegisterNumber);
        }

        public void GetFiscalTransactionStatus(bool current, out bool isOpened, out int numberOfItems, out double totalSum, out double paidSum)
        {
            var message = current ? SendAndReceive(MessageType.M76O, "T") : SendAndReceive(MessageType.M76);

            Parse(message, MessageType.M76, out char openedChar, out numberOfItems, out totalSum, out paidSum);
            isOpened = openedChar != '0';
        }

        public void CloseFiscalReceipt()
        {
            SendAndReceive(MessageType.M56);
        }

        public void CloseNonFiscalReceipt()
        {
            SendAndReceive(MessageType.M39);
        }

        public void RecordPayment(string paymentType = "", double amount = 0)
        {
            amount = Math.Round(amount, 2);
            if (amount > 0)
                SendAndReceive(MessageType.M53PA, paymentType, amount);
            else if (paymentType != string.Empty)
                SendAndReceive(MessageType.M53P, paymentType);
            else
                SendAndReceive(MessageType.M53);
        }

        public void AddItemToFiscalReceipt(int itemId, double quantity, bool showOnDisplay, bool forceTwoRowsDisplay = false)
        {
            quantity = Math.Round(quantity, 3);
            if (quantity == 0)
                return;

            MessageType messageType = showOnDisplay ? MessageType.M52Q : MessageType.M58Q;

            if (quantity == 1 && !forceTwoRowsDisplay)
                messageType = showOnDisplay ? MessageType.M52 : MessageType.M58;
            if (quantity < 0)
            {
                itemId = -itemId;
                quantity = -quantity;
            }
            SendAndReceive(messageType, itemId, quantity);
        }

        public void SetDisplayTopRowText(string text)
        {
            SendAndReceive(MessageType.M47, text);
        }

        public void SetDisplayBottomRowText(string text)
        {
            SendAndReceive(MessageType.M35, text);
        }

        public void SetItemPrice(int itemId, double price)
        {
            price = Math.Round(price, 2);
            SendAndReceive(MessageType.M107C, itemId, price);
        }

        public int FindFirstAvailableItemId()
        {
            var message = SendAndReceive(MessageType.M107X);

            Parse(message, MessageType.M107X, out int itemId);
            return itemId;
        }

        public void DeleteItem(int itemId)
        {
            SendAndReceive(MessageType.M107D, itemId);
        }

        public void GetItemsInfo(out int maxItemNameLength, out int maxNumberOfItems, out int numberOfItems)
        {
            var message = SendAndReceive(MessageType.M107I);
            Parse(message, MessageType.M107I, out maxItemNameLength, out maxNumberOfItems, out numberOfItems);
        }

        public void PrintItemsReport(PrintItemsReportOption option)
        {
            SendAndReceive(MessageType.M111, (int)option);
        }

        public void PrintCashiersReport()
        {
            SendAndReceive(MessageType.M105);
        }

        /// <summary>
        /// </summary>
        /// <param name="type"></param>
        /// <param name="forbidErasingCashiersData">Forbid erasing cashiers data.</param>
        /// <param name="eraseItemsTotalAmount">Erase total amounts per item.</param>
        public void PrintDailyFiscalReport(DailyFiscalReportType type, bool forbidErasingCashiersData, bool eraseItemsTotalAmount)
        {
            MessageType messageType = MessageType.M69O;

            if (forbidErasingCashiersData && eraseItemsTotalAmount)
                messageType = MessageType.M69ONA;
            else if (forbidErasingCashiersData && !eraseItemsTotalAmount)
                messageType = MessageType.M69ON;
            else if (!forbidErasingCashiersData && eraseItemsTotalAmount)
                messageType = MessageType.M69OA;
            SendAndReceive(messageType, (int)type);
        }

        public void PrintPeriodicTaxRateReport(DateTime from, DateTime to)
        {
            SendAndReceive(MessageType.M50SE, $"{from.Day:00}{from.Month:00}{from.Year % 100:00}", $"{to.Day:00}{to.Month:00}{to.Year % 100:00}");
        }

        public void PrintPeriodicFiscalMemoryReport(DateTime from, DateTime to)
        {
            SendAndReceive(MessageType.M79, $"{from.Day:00}{from.Month:00}{from.Year % 100:00}", $"{to.Day:00}{to.Month:00}{to.Year % 100:00}");
        }

        public void PrintFiscalMemoryReport(int startRecord, int endRecord, FiscalMemoryReportType type)
        {
            SendAndReceive(MessageType.M73, startRecord, endRecord, (int)type);
        }

        public double[] GetTotalsPerTaxRateForCurrentDay(out double total)
        {
            var message = SendAndReceive(MessageType.M65);

            Parse(message, MessageType.M65, out total, out double taxRate1, out double taxRate2, out double taxRate3, out double taxRate4, out double taxRate5, out double taxRate6, out double taxRate7, out double taxRate8, out double taxRate9);
            var taxRates = new[] { taxRate1, taxRate2, taxRate3, taxRate4, taxRate5, taxRate6, taxRate7, taxRate8, taxRate9 };
            total /= 100;
            for (int i = 0; i < taxRates.Length; i++)
                taxRates[i] /= 100;
            return taxRates;
        }

        public void GetTotalsForCurrentDay(out double total, out double totalRemoved, out double notPaid, out int totalFiscalReceipts, out int totalReceipts)
        {
            var message = SendAndReceive(MessageType.M67);

            Parse(message, MessageType.M67, out total, out totalRemoved, out notPaid, out totalFiscalReceipts, out totalReceipts);
            total /= 100;
            totalRemoved /= 100;
            notPaid /= 100;
        }

        public void GetAdditionalInfoForCurrentDay(out double cash, out double unknown1, out double card, out double check, out int unknown2, out int unknown3)
        {
            var message = SendAndReceive(MessageType.M110);

            Parse(message, MessageType.M110, out cash, out unknown1, out card, out check, out unknown2, out unknown3);
            cash /= 100;
            card /= 100;
            check /= 100;
        }

        public void OpenNonFiscalReceipt()
        {
            SendAndReceive(MessageType.M38);
        }

        public void AddNonFiscalReceiptLine(string text)
        {
            SendAndReceive(MessageType.M42, text);
        }

        public double?[] GetTaxRates(out int decimals)
        {
            var message = SendAndReceive(MessageType.M83);
            double?[] taxRates = new double?[9];
            Parse(message, MessageType.M83, out decimals, out string flags, out double taxRate1, out double taxRate2, out double taxRate3, out double taxRate4, out double taxRate5, out double taxRate6, out double taxRate7, out double taxRate8, out double taxRate9);

            if (flags[0] != '0')
                taxRates[0] = taxRate1;
            if (flags[1] != '0')
                taxRates[1] = taxRate2;
            if (flags[2] != '0')
                taxRates[2] = taxRate3;
            if (flags[3] != '0')
                taxRates[3] = taxRate4;
            if (flags[4] != '0')
                taxRates[4] = taxRate5;
            if (flags[5] != '0')
                taxRates[5] = taxRate6;
            if (flags[6] != '0')
                taxRates[6] = taxRate7;
            if (flags[7] != '0')
                taxRates[7] = taxRate8;
            if (flags[8] != '0')
                taxRates[8] = taxRate9;
            return taxRates;
        }

        public string GetHeaderFooterText(int line)
        {
            var message = SendAndReceive(MessageType.M43, "I", line);

            Parse(message, MessageType.M43, out string text);
            return text;
        }

        public void SetHeaderFooterText(int line, string text)
        {
            SendAndReceive(MessageType.M43, line, text);
        }

        public void AdjustSummerTime(bool offsetByHourInfront)
        {
            SendAndReceive(MessageType.M60, offsetByHourInfront ? 1 : 0);
        }

        public void SetDateTime(DateTime dateTime)
        {
            SendAndReceive(MessageType.M61S, dateTime);
        }

        public void SetLogoOnFiscalReceipt(bool showLogo)
        {
            SendAndReceive(MessageType.M43, "L", showLogo ? 1 : 0);
        }

        public bool GetLogoOnFiscalReceipt()
        {
            var message = SendAndReceive(MessageType.M43, "I", "L");

            Parse(message, MessageType.M43, out char showLogo);
            return showLogo != '0';
        }

        public void UpdateLogoLine(int line, string encodedText)
        {
            SendAndReceive(MessageType.M115, line, encodedText);
        }

        public void SetTaxRates(int decimals, double?[] taxRates)
        {
            StringBuilder flags = new StringBuilder(9);
            for (int i = 0; i < taxRates.Length; i++)
                flags.Append(taxRates[i] != null ? '1' : '0');
            SendAndReceive(MessageType.M83DFX, flags.ToString(), taxRates[0] ?? 0, taxRates[1] ?? 0, taxRates[2] ?? 0, taxRates[3] ?? 0, taxRates[4] ?? 0, taxRates[5] ?? 0, taxRates[6] ?? 0, taxRates[7] ?? 0, taxRates[8] ?? 0);
        }

        public void UpdateCachierName(string cachier, string password, string name)
        {
            SendAndReceive(MessageType.M102, cachier, password, name);
        }

        public void AddItem(string taxRate, int itemId, double price, string name)
        {
            SendAndReceive(MessageType.M107P, taxRate, itemId, price, name);
        }

        public bool GetItem(ref int itemId, out string taxRate, out double price, out double quantity, out string name)
        {
            var message = SendAndReceive(MessageType.M107R, itemId);

            if (message.Data.Count == 1 && (message.Data.AsSpan()[0] == (byte)'F' || message.Data.AsSpan()[0] == (byte)'N'))
            {
                itemId = 0;
                taxRate = string.Empty;
                price = 0;
                quantity = 0;
                name = string.Empty;
                return false;
            }
            Parse(message, MessageType.M107R, out string _, out itemId, out taxRate, out price, out quantity, out name);
            return true;
        }

        public bool GetFirstItem(out int itemId, out string taxRate, out double price, out double quantity, out string name)
        {
            var message = SendAndReceive(MessageType.M107F);

            if (message.Data.Count == 1 && message.Data.AsSpan()[0] == (byte)'F')
            {
                itemId = 0;
                taxRate = string.Empty;
                price = 0;
                quantity = 0;
                name = string.Empty;
                return false;
            }
            Parse(message, MessageType.M107F, out string _, out itemId, out taxRate, out price, out quantity, out name);
            return true;
        }

        public bool GetNextItem(out int itemId, out string taxRate, out double price, out double quantity, out string name)
        {
            var message = SendAndReceive(MessageType.M107N);

            if (message.Data.Count == 1 && message.Data.AsSpan()[0] == (byte)'F')
            {
                itemId = 0;
                taxRate = string.Empty;
                price = 0;
                quantity = 0;
                name = string.Empty;
                return false;
            }
            Parse(message, MessageType.M107N, out string _, out itemId, out taxRate, out price, out quantity, out name);
            return true;
        }

        protected virtual bool ShouldRetryMessage(Status printerStatus)
        {
            return false;
        }

        private void Check(ReceivedPrinterMessage message)
        {
            if (message.PrinterStatus.HasError)
                throw new PrinterStatusException(message.PrinterStatus);
        }

        private ReceivedPrinterMessage SendAndReceiveNoCheck(MessageType message, params object[] parameters)
        {
            programCommunication.SendMessage(message, NextMessageId, parameters);
            return printerCommunication.ReceiveMessage();
        }

        private ReceivedPrinterMessage SendAndReceive(MessageType message, params object[] parameters)
        {
            while (true)
            {
                ReceivedPrinterMessage receivedMessage = SendAndReceiveNoCheck(message, parameters);

                if (ShouldRetryMessage(receivedMessage.PrinterStatus))
                    continue;
                Check(receivedMessage);
                return receivedMessage;
            }
        }

        #region Parsing messages
        private void Parse<T>(ReceivedPrinterMessage receivedMessage, MessageType messageType, out T value)
        {
            var parameter = new Parameter<T>();
            Parse(receivedMessage, messageType, parameter);
            value = parameter.Value;
        }

        private void Parse<T1, T2>(ReceivedPrinterMessage receivedMessage, MessageType messageType, out T1 value1, out T2 value2)
        {
            var parameter1 = new Parameter<T1>();
            var parameter2 = new Parameter<T2>();
            Parse(receivedMessage, messageType, parameter1, parameter2);
            value1 = parameter1.Value;
            value2 = parameter2.Value;
        }

        private void Parse<T1, T2, T3>(ReceivedPrinterMessage receivedMessage, MessageType messageType, out T1 value1, out T2 value2, out T3 value3)
        {
            var parameter1 = new Parameter<T1>();
            var parameter2 = new Parameter<T2>();
            var parameter3 = new Parameter<T3>();
            Parse(receivedMessage, messageType, parameter1, parameter2, parameter3);
            value1 = parameter1.Value;
            value2 = parameter2.Value;
            value3 = parameter3.Value;
        }

        private void Parse<T1, T2, T3, T4>(ReceivedPrinterMessage receivedMessage, MessageType messageType, out T1 value1, out T2 value2, out T3 value3, out T4 value4)
        {
            var parameter1 = new Parameter<T1>();
            var parameter2 = new Parameter<T2>();
            var parameter3 = new Parameter<T3>();
            var parameter4 = new Parameter<T4>();
            Parse(receivedMessage, messageType, parameter1, parameter2, parameter3, parameter4);
            value1 = parameter1.Value;
            value2 = parameter2.Value;
            value3 = parameter3.Value;
            value4 = parameter4.Value;
        }

        private void Parse<T1, T2, T3, T4, T5>(ReceivedPrinterMessage receivedMessage, MessageType messageType, out T1 value1, out T2 value2, out T3 value3, out T4 value4, out T5 value5)
        {
            var parameter1 = new Parameter<T1>();
            var parameter2 = new Parameter<T2>();
            var parameter3 = new Parameter<T3>();
            var parameter4 = new Parameter<T4>();
            var parameter5 = new Parameter<T5>();
            Parse(receivedMessage, messageType, parameter1, parameter2, parameter3, parameter4, parameter5);
            value1 = parameter1.Value;
            value2 = parameter2.Value;
            value3 = parameter3.Value;
            value4 = parameter4.Value;
            value5 = parameter5.Value;
        }

        private void Parse<T1, T2, T3, T4, T5, T6>(ReceivedPrinterMessage receivedMessage, MessageType messageType, out T1 value1, out T2 value2, out T3 value3, out T4 value4, out T5 value5, out T6 value6)
        {
            var parameter1 = new Parameter<T1>();
            var parameter2 = new Parameter<T2>();
            var parameter3 = new Parameter<T3>();
            var parameter4 = new Parameter<T4>();
            var parameter5 = new Parameter<T5>();
            var parameter6 = new Parameter<T6>();
            Parse(receivedMessage, messageType, parameter1, parameter2, parameter3, parameter4, parameter5, parameter6);
            value1 = parameter1.Value;
            value2 = parameter2.Value;
            value3 = parameter3.Value;
            value4 = parameter4.Value;
            value5 = parameter5.Value;
            value6 = parameter6.Value;
        }

        private void Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ReceivedPrinterMessage receivedMessage, MessageType messageType, out T1 value1, out T2 value2, out T3 value3, out T4 value4, out T5 value5, out T6 value6, out T7 value7, out T8 value8, out T9 value9, out T10 value10)
        {
            var parameter1 = new Parameter<T1>();
            var parameter2 = new Parameter<T2>();
            var parameter3 = new Parameter<T3>();
            var parameter4 = new Parameter<T4>();
            var parameter5 = new Parameter<T5>();
            var parameter6 = new Parameter<T6>();
            var parameter7 = new Parameter<T7>();
            var parameter8 = new Parameter<T8>();
            var parameter9 = new Parameter<T9>();
            var parameter10 = new Parameter<T10>();
            Parse(receivedMessage, messageType, parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9, parameter10);
            value1 = parameter1.Value;
            value2 = parameter2.Value;
            value3 = parameter3.Value;
            value4 = parameter4.Value;
            value5 = parameter5.Value;
            value6 = parameter6.Value;
            value7 = parameter7.Value;
            value8 = parameter8.Value;
            value9 = parameter9.Value;
            value10 = parameter10.Value;
        }

        private void Parse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ReceivedPrinterMessage receivedMessage, MessageType messageType, out T1 value1, out T2 value2, out T3 value3, out T4 value4, out T5 value5, out T6 value6, out T7 value7, out T8 value8, out T9 value9, out T10 value10, out T11 value11)
        {
            var parameter1 = new Parameter<T1>();
            var parameter2 = new Parameter<T2>();
            var parameter3 = new Parameter<T3>();
            var parameter4 = new Parameter<T4>();
            var parameter5 = new Parameter<T5>();
            var parameter6 = new Parameter<T6>();
            var parameter7 = new Parameter<T7>();
            var parameter8 = new Parameter<T8>();
            var parameter9 = new Parameter<T9>();
            var parameter10 = new Parameter<T10>();
            var parameter11 = new Parameter<T11>();
            Parse(receivedMessage, messageType, parameter1, parameter2, parameter3, parameter4, parameter5, parameter6, parameter7, parameter8, parameter9, parameter10, parameter11);
            value1 = parameter1.Value;
            value2 = parameter2.Value;
            value3 = parameter3.Value;
            value4 = parameter4.Value;
            value5 = parameter5.Value;
            value6 = parameter6.Value;
            value7 = parameter7.Value;
            value8 = parameter8.Value;
            value9 = parameter9.Value;
            value10 = parameter10.Value;
            value11 = parameter11.Value;
        }

        private void Parse(ReceivedPrinterMessage receivedMessage, MessageType messageType, params object[] parameters)
        {
            printerCommunication.ParseDataMessage(receivedMessage.Data, messageType, parameters, fromPrinter: true);
        }
        #endregion
    }
}
