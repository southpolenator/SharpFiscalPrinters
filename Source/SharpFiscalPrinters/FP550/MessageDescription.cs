using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpFiscalPrinters.FP550
{
    internal struct MessageDescription
    {
        public MessageType MessageType;
        public string ProgramFormat;
        public string PrinterFormat;
        public byte MessageNumber;

        private static Dictionary<MessageType, MessageDescription> messageTypes;

        static MessageDescription()
        {
            messageTypes = new Dictionary<MessageType, MessageDescription>();
            var enumType = typeof(MessageType);
#pragma warning disable CS8605 // Unboxing a possibly null value.
            foreach (MessageType messageType in Enum.GetValues(enumType))
#pragma warning restore CS8605 // Unboxing a possibly null value.
            {
                var memberInfos = enumType.GetMember(messageType.ToString());
                var enumValueMemberInfo = memberInfos.FirstOrDefault(m => m.DeclaringType == enumType);
                var programFormatAttribute = enumValueMemberInfo.GetCustomAttributes(typeof(ProgramFormatAttribute), false).FirstOrDefault() as ProgramFormatAttribute;
                var printerFormatAttribute = enumValueMemberInfo.GetCustomAttributes(typeof(PrinterFormatAttribute), false).FirstOrDefault() as PrinterFormatAttribute;
                var messageNumberAttribute = enumValueMemberInfo.GetCustomAttributes(typeof(MessageNumberAttribute), false).FirstOrDefault() as MessageNumberAttribute;

                messageTypes.Add(messageType, new MessageDescription()
                {
                    MessageType = messageType,
                    ProgramFormat = programFormatAttribute?.Format ?? string.Empty,
                    PrinterFormat = printerFormatAttribute?.Format ?? string.Empty,
                    MessageNumber = messageNumberAttribute?.MessageNumber ?? 0,
                });
            }
        }

        public static MessageDescription Find(MessageType messageType)
        {
            return messageTypes[messageType];
        }
    }
}
