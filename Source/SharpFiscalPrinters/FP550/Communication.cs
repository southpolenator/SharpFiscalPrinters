using SharpFiscalPrinters.FP550.Exceptions;
using System;
using System.Linq;
using System.Text;

namespace SharpFiscalPrinters.FP550
{
    internal class Communication
    {
        public const byte MinimumMessageId = 32;
        public const byte MaximumMessageId = 127;
        private byte[] dataMessageBuffer;
        private byte[] fullMessageBuffer;
        private Encoding? stringEncoding = null;

        public Communication(IPort port, Encoding? stringEncoding = null)
        {
            Port = port;
            this.stringEncoding = stringEncoding;
            dataMessageBuffer = new byte[1024];
            fullMessageBuffer = new byte[1024];
        }

        public IPort Port { get; private set; }

        public void SendMessage(MessageType message, byte messageId, params object[] parameters)
        {
            Port.WriteBytes(GenerateMessage(message, messageId, parameters));
        }

        public ReceivedMessage ReceiveMessage(bool fromPrinter = true)
        {
            // Wait for message start
            for (int retryCount = 0; ; retryCount++)
            {
                fullMessageBuffer[0] = Port.ReadByte();

                // Check if message sent to the printer was ill-formed
                if (fullMessageBuffer[0] == 21)
                    throw new IllFormedMessageException("Invalid message was sent to printer");

                // Check if printer is busy
                if (fullMessageBuffer[0] == 22)
                    continue;

                // Ok, we've received message start
                break;
            }
            if (fullMessageBuffer[0] != 1)
                throw new IllFormedMessageException("Unexpected message start");

            // Read message header
            int fullLength = 1;

            fullMessageBuffer[fullLength++] = Port.ReadByte();
            fullMessageBuffer[fullLength++] = Port.ReadByte();
            fullMessageBuffer[fullLength++] = Port.ReadByte();

            int dataLength = fullMessageBuffer[1] - 36;
            ReceivedMessage receivedMessage = new ReceivedMessage();
            receivedMessage.MessageId = fullMessageBuffer[2];
            receivedMessage.MessageNumber = fullMessageBuffer[3];

            // If printer is sending message, it is returning status too.
            if (fromPrinter)
                dataLength -= 1 + Status.BytesCount;

            // Read message data
            for (int i = 0; i < dataLength; i++)
                fullMessageBuffer[fullLength++] = Port.ReadByte();

            // If printer is sending message, read status
            if (fromPrinter)
            {
                fullMessageBuffer[fullLength++] = Port.ReadByte();
                if (fullMessageBuffer[fullLength - 1] != 4)
                    throw new IllFormedMessageException("Expected printer status");
                for (int i = 0; i < Status.BytesCount; i++)
                    fullMessageBuffer[fullLength++] = Port.ReadByte();
                receivedMessage.PrinterStatus = new Status(fullMessageBuffer.AsSpan(fullLength - Status.BytesCount, Status.BytesCount));
            }

            // Read message end
            fullMessageBuffer[fullLength++] = Port.ReadByte();
            if (fullMessageBuffer[fullLength - 1] != 5)
                throw new IllFormedMessageException("Unexpected data message end");
            for (int i = 0; i < 4; i++)
                fullMessageBuffer[fullLength++] = Port.ReadByte();
            fullMessageBuffer[fullLength++] = Port.ReadByte();
            if (fullMessageBuffer[fullLength - 1] != 3)
                throw new IllFormedMessageException("Unexpected message end");

            // Verify BCC
            Span<byte> bcc = stackalloc byte[4];

            receivedMessage.Data = new ArraySegment<byte>(fullMessageBuffer, 4, dataLength);
            WriteBcc(bcc, fullMessageBuffer.AsSpan(0, 4), fullMessageBuffer.AsSpan(4, fullLength - 10));
            for (int i = 0; i < bcc.Length; i++)
                if (bcc[i] != fullMessageBuffer[fullLength - 5 + i])
                    throw new IllFormedMessageException("Invalid BCC");

            // Return received message
            receivedMessage.Bytes = new ArraySegment<byte>(fullMessageBuffer, 0, fullLength);
            return receivedMessage;
        }

        internal ArraySegment<byte> GenerateMessage(MessageType message, byte messageId, params object[] data)
        {
            return GenerateMessage(message, messageId, null, data);
        }

        internal ArraySegment<byte> GenerateMessage(MessageType message, byte messageId, Status? printerStatus, params object[] data)
        {
            var dataMessage = CreateDataMessage(dataMessageBuffer, message, data, printerStatus != null);

            return CreateFullMessage(fullMessageBuffer, message, messageId, dataMessage, printerStatus);
        }

        internal static ArraySegment<byte> CreateFullMessage(byte[] messageBuffer, MessageType message, byte messageId, ArraySegment<byte> dataMessage, Status? printerStatus)
        {
            int length = 0;

            messageBuffer[length++] = 1;
            messageBuffer[length++] = (byte)(36 + dataMessage.Count);
            messageBuffer[length++] = messageId;
            messageBuffer[length++] = MessageDescription.Find(message).MessageNumber;
            for (int i = 0; i < dataMessage.Count; i++)
                messageBuffer[length++] = dataMessage.Array[dataMessage.Offset + i];
            if (printerStatus != null)
            {
                messageBuffer[length++] = 4;
                printerStatus.Value.WriteToBuffer(messageBuffer.AsSpan(length));
                length += Status.BytesCount;
            }
            messageBuffer[length++] = 5;
            WriteBcc(messageBuffer.AsSpan(length), messageBuffer.AsSpan(0, 4), dataMessage);
            length += 4;
            messageBuffer[length++] = 3;
            return new ArraySegment<byte>(messageBuffer, 0, length);
        }

        internal ArraySegment<byte> CreateDataMessage(byte[] messageBuffer, MessageType message, object[] parameters, bool fromPrinter)
        {
            int length = 0;
            MessageDescription messageDescription = MessageDescription.Find(message);
            string messageFormat = fromPrinter ? messageDescription.PrinterFormat : messageDescription.ProgramFormat;
            int parameterIndex = 0;

            foreach (char c in messageFormat)
            {
                switch (c)
                {
                    case 'W':
                        int a = Convert.ToInt32(parameters[parameterIndex++]);

                        for (int i = 0; i < 4; i++, a /= 16)
                            messageBuffer[length + 3 - i] = (byte)((a % 16 >= 10) ? (a % 16 - 10 + 'A') : (a % 16 + '0'));
                        length += 4;
                        break;
                    case 'B':
                        string number = parameters[parameterIndex++]?.ToString() ?? string.Empty;

                        foreach (var cc in ToBytes(number))
                            messageBuffer[length++] = cc;
                        break;
                    case 'T':
                        string text = parameters[parameterIndex++]?.ToString() ?? string.Empty;

                        foreach (var cc in ToBytes(text))
                            if (cc >= 0 && cc < 32)
                            {
                                messageBuffer[length++] = 16;
                                messageBuffer[length++] = (byte)(cc + 64);
                            }
                            else
                                messageBuffer[length++] = cc;
                        break;
                    case 'Q':
                        {
                            DateTime dt = (DateTime)parameters[parameterIndex++];

                            messageBuffer[length++] = (byte)(dt.Day / 10 + '0');
                            messageBuffer[length++] = (byte)(dt.Day % 10 + '0');
                            messageBuffer[length++] = (byte)'-';
                            messageBuffer[length++] = (byte)(dt.Month / 10 + '0');
                            messageBuffer[length++] = (byte)(dt.Month % 10 + '0');
                            messageBuffer[length++] = (byte)'-';
                            messageBuffer[length++] = (byte)(dt.Year % 100 / 10 + '0');
                            messageBuffer[length++] = (byte)(dt.Year % 10 + '0');
                            messageBuffer[length++] = (byte)' ';
                            messageBuffer[length++] = (byte)(dt.Hour / 10 + '0');
                            messageBuffer[length++] = (byte)(dt.Hour % 10 + '0');
                            messageBuffer[length++] = (byte)'-';
                            messageBuffer[length++] = (byte)(dt.Minute / 10 + '0');
                            messageBuffer[length++] = (byte)(dt.Minute % 10 + '0');
                        }
                        break;
                    case 'q':
                        {
                            DateTime dt = (DateTime)parameters[parameterIndex++];

                            messageBuffer[length++] = (byte)(dt.Day / 10 + '0');
                            messageBuffer[length++] = (byte)(dt.Day % 10 + '0');
                            messageBuffer[length++] = (byte)'-';
                            messageBuffer[length++] = (byte)(dt.Month / 10 + '0');
                            messageBuffer[length++] = (byte)(dt.Month % 10 + '0');
                            messageBuffer[length++] = (byte)'-';
                            messageBuffer[length++] = (byte)(dt.Year % 100 / 10 + '0');
                            messageBuffer[length++] = (byte)(dt.Year % 10 + '0');
                            messageBuffer[length++] = (byte)' ';
                            messageBuffer[length++] = (byte)(dt.Hour / 10 + '0');
                            messageBuffer[length++] = (byte)(dt.Hour % 10 + '0');
                            messageBuffer[length++] = (byte)'-';
                            messageBuffer[length++] = (byte)(dt.Minute / 10 + '0');
                            messageBuffer[length++] = (byte)(dt.Minute % 10 + '0');
                            messageBuffer[length++] = (byte)'-';
                            messageBuffer[length++] = (byte)(dt.Second / 10 + '0');
                            messageBuffer[length++] = (byte)(dt.Second % 10 + '0');
                        }
                        break;
                    default:
                        messageBuffer[length++] = (byte)c;
                        break;


                    // TODO: Remove this as it looks like it is not used!!!
                    case 'Y':
                        // TODO: Use DateTime structure instead of three arguments
                        int day = Convert.ToInt32(parameters[parameterIndex++]);
                        int month = Convert.ToInt32(parameters[parameterIndex++]);
                        int year = Convert.ToInt32(parameters[parameterIndex++]);

                        messageBuffer[length++] = (byte)(day / 10 + '0');
                        messageBuffer[length++] = (byte)(day % 10 + '0');
                        messageBuffer[length++] = (byte)'0';
                        messageBuffer[length++] = (byte)(month / 10 + '0');
                        messageBuffer[length++] = (byte)(month % 10 + '0');
                        messageBuffer[length++] = (byte)(year / 10 + '0');
                        messageBuffer[length++] = (byte)(year % 10 + '0');
                        break;
                    case 'Z':
                        // TODO: Use DateTime structure instead of two arguments
                        int hour = Convert.ToInt32(parameters[parameterIndex++]);
                        int minute = Convert.ToInt32(parameters[parameterIndex++]);

                        messageBuffer[length++] = (byte)(hour / 10 + '0');
                        messageBuffer[length++] = (byte)(hour % 10 + '0');
                        messageBuffer[length++] = (byte)(minute / 10 + '0');
                        messageBuffer[length++] = (byte)(minute % 10 + '0');
                        break;
                }
            }
            return new ArraySegment<byte>(messageBuffer, 0, length);
        }

        internal void ParseDataMessage(ArraySegment<byte> dataMessage, MessageType message, object[] parameters, bool fromPrinter)
        {
            ReadOnlySpan<byte> buffer = dataMessage.AsSpan();
            int dataIndex = 0;
            MessageDescription messageDescription = MessageDescription.Find(message);
            string messageFormat = fromPrinter ? messageDescription.PrinterFormat : messageDescription.ProgramFormat;
            int parameterIndex = 0;

            for (int formatIndex = 0; formatIndex < messageFormat.Length; formatIndex++)
            {
                char c = messageFormat[formatIndex];

                if (c == 'T' || c == 'B' || c == 'W' || c == 'Z' || c == 'Y' || c == 'Q' || c == 'q')
                {
                    ReadOnlySpan<byte> parameterData;

                    // If it is last token, take everything until the end.
                    if (formatIndex + 1 == messageFormat.Length)
                        parameterData = buffer.Slice(dataIndex);
                    else
                    {
                        char nc = messageFormat[formatIndex + 1];

                        // If next token is text or number, we know use known size
                        if (nc == 'T' || nc == 'B' || nc == 'W' || nc == 'Z' || nc == 'Y')
                        {
                            if (c == 'W')
                                parameterData = buffer.Slice(dataIndex, 4);
                            else if (c == 'Y')
                                parameterData = buffer.Slice(dataIndex, 7);
                            else if (c == 'Z')
                                parameterData = buffer.Slice(dataIndex, 4);
                            else
                                parameterData = buffer.Slice(dataIndex, 1);
                        }
                        else
                        {
                            byte endByte = (byte)nc;

                            parameterData = buffer.Slice(dataIndex);
                            for (int i = 0; i < parameterData.Length; i++)
                                if (parameterData[i] == endByte)
                                {
                                    parameterData = parameterData.Slice(0, i);
                                    break;
                                }
                        }
                    }
                    dataIndex += parameterData.Length;

                    switch (c)
                    {
                        case 'W':
                            {
                                int number = 0;

                                for (int i = 0; i < 4; i++)
                                    number = number * 16 + ((parameterData[i] >= 'A') ? (parameterData[i] - 'A' + 10) : (parameterData[i] - '0'));
                                Store(number, parameters[parameterIndex++]);
                            }
                            break;
                        case 'T':
                        case 'B':
                            byte[] stringBytes = new byte[parameterData.Length];
                            int stringBytesUsed = 0;
                            for (int i = 0; i < parameterData.Length; i++)
                                if (parameterData[i] == 16)
                                    stringBytes[stringBytesUsed++] = (byte)(parameterData[++i] + 64);
                                else
                                    stringBytes[stringBytesUsed++] = parameterData[i];
                            var s = ToString(stringBytes, stringBytesUsed);
                            if (c == 'T')
                                Store(s, parameters[parameterIndex++]);
                            else
                                Store(double.Parse(s), parameters[parameterIndex++]);
                            break;
                        case 'Q':
                            // TODO: Implement this
                            break;
                        case 'q':
                            // TODO: Implement this
                            break;



                        // TODO: Remove this as it looks like it is not used!!!
                        case 'Y':
                            {
                                int day, month, year;

                                day = parameterData[0] - '0' * 10 + parameterData[1] - '0';
                                month = parameterData[3] - '0' * 10 + parameterData[4] - '0';
                                year = parameterData[5] - '0' * 10 + parameterData[6] - '0';
                                Store(day, parameters[parameterIndex++]);
                                Store(month, parameters[parameterIndex++]);
                                Store(year, parameters[parameterIndex++]);
                            }
                            break;
                        case 'Z':
                            {
                                int hours, minutes;

                                hours = parameterData[0] - '0' * 10 + parameterData[1] - '0';
                                minutes = parameterData[2] - '0' * 10 + parameterData[3] - '0';
                                Store(hours, parameters[parameterIndex++]);
                                Store(minutes, parameters[parameterIndex++]);
                            }
                            break;
                    }
                }
                else
                {
                    byte expectedByte = (byte)c;

                    if (buffer[dataIndex] != expectedByte)
                        throw new IllFormedMessageException($"Encountered byte {buffer[dataIndex]} [{(char)buffer[dataIndex]}] while expecting {expectedByte} [{(char)expectedByte}]");
                    dataIndex++;
                }
            }
        }

        private unsafe string ToString(byte[] bytes, int length)
        {
            if (stringEncoding != null)
                return stringEncoding.GetString(bytes, 0, length);
            fixed (byte* b = bytes)
            {
                return new string((sbyte*)b, 0, length);
            }
        }

        private byte[] ToBytes(string text)
        {
            if (stringEncoding != null)
                return stringEncoding.GetBytes(text);
            byte[] bytes = new byte[text.Length];
            for (int i = 0; i < text.Length; i++)
                bytes[i] = (byte)text[i];
            return bytes;
        }

        private static void WriteBcc(Span<byte> buffer, ReadOnlySpan<byte> messageHeader, ReadOnlySpan<byte> dataMessage)
        {
            ushort bcc = GetDataMessageBcc(messageHeader, dataMessage);

            WriteBcc(buffer, bcc);
        }

        private static ushort GetDataMessageBcc(ReadOnlySpan<byte> messageHeader, ReadOnlySpan<byte> dataMessage)
        {
            ushort sum = 4;

            for (int i = 0; i < messageHeader.Length; i++)
                sum += messageHeader[i];
            for (int i = 0; i < dataMessage.Length; i++)
                sum += dataMessage[i];
            return sum;
        }

        private static void WriteBcc(Span<byte> buffer, ushort value)
        {
            ushort a1 = (ushort)(value / 256);
            ushort a2 = (ushort)(value % 256);

            buffer[0] = (byte)(a1 / 16 + 48);
            buffer[1] = (byte)(a1 % 16 + 48);
            buffer[2] = (byte)(a2 / 16 + 48);
            buffer[3] = (byte)(a2 % 16 + 48);
        }

        private static void Store(int value, object parameter)
        {
            if (parameter is Parameter<int> intParameter)
                intParameter.Value = value;
            else if (parameter is Parameter<double> doubleParameter)
                doubleParameter.Value = value;
            else
                throw new Exception($"Unexpected parameter type [{parameter.GetType().Name}] while storing int");
        }

        private static void Store(double value, object parameter)
        {
            if (parameter is Parameter<int> intParameter)
                intParameter.Value = (int)Math.Round(value);
            else if (parameter is Parameter<double> doubleParameter)
                doubleParameter.Value = value;
            else
                throw new Exception($"Unexpected parameter type [{parameter.GetType().Name}] while storing double");
        }

        private static void Store(string value, object parameter)
        {
            if (parameter is Parameter<string> stringParameter)
                stringParameter.Value = value;
            else if (parameter is Parameter<char> charParameter)
                charParameter.Value = value.Single();
            else
                throw new Exception($"Unexpected parameter type [{parameter.GetType().Name}] while storing string");
        }
    }
}
