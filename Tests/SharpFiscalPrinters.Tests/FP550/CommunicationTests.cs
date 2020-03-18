using SharpFiscalPrinters.FP550;
using System.Text;
using Xunit;

namespace SharpFiscalPrinters.Tests.FP550
{
    public class CommunicationTests
    {
        [Fact]
        public void GenerateEmptyMessage()
        {
            var communication = new Communication(EmptyPort.Instance);
            var message = communication.GenerateMessage(MessageType.M33, 32);

            Assert.Equal(10, message.Count);
            Assert.Equal(1, message[0]);
            Assert.Equal(36, message[1]);
            Assert.Equal(32, message[2]);
            Assert.Equal(33, message[3]);
            Assert.Equal(5, message[4]);
            Assert.Equal(48, message[5]);
            Assert.Equal(48, message[6]);
            Assert.Equal(54, message[7]);
            Assert.Equal(58, message[8]);
            Assert.Equal(3, message[9]);
        }

        [Fact]
        public void GenerateTestMessage()
        {
            var communication = new Communication(EmptyPort.Instance);
            var message = communication.GenerateMessage(MessageType.M74O, 60, "X");

            Assert.Equal(11, message.Count);
            Assert.Equal(0x01, message[0]);
            Assert.Equal(0x25, message[1]);
            Assert.Equal(0x3C, message[2]);
            Assert.Equal(0x4A, message[3]);
            Assert.Equal(0x58, message[4]);
            Assert.Equal(0x05, message[5]);
            Assert.Equal(0x30, message[6]);
            Assert.Equal(0x31, message[7]);
            Assert.Equal(0x30, message[8]);
            Assert.Equal(0x38, message[9]);
            Assert.Equal(0x03, message[10]);
        }

        [Fact]
        public void ParseTestMessage()
        {
            var communication = CreateParseCommunication(
                0x01, 0x31, 0x3c, 0x4a, 0x80, 0x80, 0x80, 0x85, 0x80, 0xba, 0x04, 0x80, 0x80, 0x80, 0x85, 0x80,
                0xba, 0x05, 0x30, 0x37, 0x33, 0x3e, 0x03);
            var receivedMessage = communication.ReceiveMessage();
            var status = new Status(receivedMessage.Data);

            Assert.Equal(60, receivedMessage.MessageId);
            Assert.Equal(74, receivedMessage.MessageNumber);
            Assert.Equal(Status.BytesCount, receivedMessage.Data.Count);
            Assert.Equal(receivedMessage.PrinterStatus, status);
        }

        [Fact]
        public void GenerateTestMessage2()
        {
            var communication = new Communication(EmptyPort.Instance);
            var message = communication.GenerateMessage(MessageType.M90, 61, "1");

            Assert.Equal(11, message.Count);
            Assert.Equal(0x01, message[0]);
            Assert.Equal(0x25, message[1]);
            Assert.Equal(0x3D, message[2]);
            Assert.Equal(0x5A, message[3]);
            Assert.Equal(0x31, message[4]);
            Assert.Equal(0x05, message[5]);
            Assert.Equal(0x30, message[6]);
            Assert.Equal(0x30, message[7]);
            Assert.Equal(0x3F, message[8]);
            Assert.Equal(0x32, message[9]);
            Assert.Equal(0x03, message[10]);
        }

        [Fact]
        public void ParseTestMessage2()
        {
            var communication = CreateParseCommunication(
                0x01, 0x5C, 0x3D, 0x5a, 0x34, 0x2E, 0x32, 0x32, 0x53, 0x52, 0x20, 0x32, 0x30, 0x46, 0x45, 0x42,
                0x31, 0x33, 0x20, 0x31, 0x30, 0x30, 0x30, 0x2C, 0x38, 0x41, 0x33, 0x33, 0x2C, 0x31, 0x30, 0x31,
                0x30, 0x2C, 0x38, 0x2C, 0xC0, 0xa3, 0x30, 0x37, 0x34, 0x30, 0x33, 0x30, 0x2c, 0x30, 0x30, 0x30,
                0x30, 0x30, 0x30, 0x30, 0x30, 0x04, 0x80, 0x80, 0x80, 0x85, 0x80, 0xba, 0x05, 0x30, 0x3f, 0x30,
                0x30, 0x03);
            var receivedMessage = communication.ReceiveMessage();

            Assert.Equal(61, receivedMessage.MessageId);
            Assert.Equal(90, receivedMessage.MessageNumber);
            Assert.Equal(49, receivedMessage.Data.Count);

            var s1 = new Parameter<string>();
            var s2 = new Parameter<string>();
            var s3 = new Parameter<string>();
            var s4 = new Parameter<string>();
            var s5 = new Parameter<string>();
            var s6 = new Parameter<string>();

            communication.ParseDataMessage(receivedMessage.Data, MessageType.M90, new[] { s1, s2, s3, s4, s5, s6 }, fromPrinter: true);

            Assert.Equal("4.22SR 20FEB13 1000", s1.Value);
            Assert.Equal("8A33", s2.Value);
            Assert.Equal("1010", s3.Value);
            Assert.Equal("8", s4.Value);
            Assert.Equal("АЈ074030", s5.Value);
            Assert.Equal("00000000", s6.Value);
        }

        [Fact]
        public void GenerateTestMessage3()
        {
            var communication = new Communication(EmptyPort.Instance);
            var message = communication.GenerateMessage(MessageType.M99, 62);

            Assert.Equal(10, message.Count);
            Assert.Equal(0x01, message[0]);
            Assert.Equal(0x24, message[1]);
            Assert.Equal(0x3E, message[2]);
            Assert.Equal(0x63, message[3]);
            Assert.Equal(0x05, message[4]);
            Assert.Equal(0x30, message[5]);
            Assert.Equal(0x30, message[6]);
            Assert.Equal(0x3C, message[7]);
            Assert.Equal(0x3A, message[8]);
            Assert.Equal(0x03, message[9]);
        }

        [Fact]
        public void ParseTestMessage3()
        {
            var communication = CreateParseCommunication(
                0x01, 0x38, 0x3E, 0x63, 0x31, 0x30, 0x30, 0x30, 0x37, 0x37, 0x35, 0x35, 0x36, 0x2c, 0xcf, 0xc8,
                0xc1, 0x04, 0x80, 0x80, 0x80, 0x85, 0x80, 0xba, 0x05, 0x30, 0x38, 0x37, 0x34, 0x03);
            var receivedMessage = communication.ReceiveMessage();

            Assert.Equal(62, receivedMessage.MessageId);
            Assert.Equal(99, receivedMessage.MessageNumber);
            Assert.Equal(13, receivedMessage.Data.Count);

            var s1 = new Parameter<string>();
            var s2 = new Parameter<string>();

            communication.ParseDataMessage(receivedMessage.Data, MessageType.M99, new[] { s1, s2 }, fromPrinter: true);

            Assert.Equal("100077556", s1.Value);
            Assert.Equal("ПИБ", s2.Value);
        }

        [Fact]
        public void GenerateTestMessage4()
        {
            var communication = new Communication(EmptyPort.Instance);
            var message = communication.GenerateMessage(MessageType.M74O, 63, "X");

            Assert.Equal(11, message.Count);
            Assert.Equal(0x01, message[0]);
            Assert.Equal(0x25, message[1]);
            Assert.Equal(0x3F, message[2]);
            Assert.Equal(0x4A, message[3]);
            Assert.Equal(0x58, message[4]);
            Assert.Equal(0x05, message[5]);
            Assert.Equal(0x30, message[6]);
            Assert.Equal(0x31, message[7]);
            Assert.Equal(0x30, message[8]);
            Assert.Equal(0x3B, message[9]);
            Assert.Equal(0x03, message[10]);
        }

        [Fact]
        public void GenerateMessageWithNumberParameter()
        {
            var communication = new Communication(EmptyPort.Instance);
            var message = communication.GenerateMessage(MessageType.M44L, 32, 3.29);

            Assert.Equal(14, message.Count);
            Assert.Equal(1, message[0]);
            Assert.Equal(40, message[1]);
            Assert.Equal(32, message[2]);
            Assert.Equal(44, message[3]);
            Assert.Equal((byte)'3', message[4]);
            Assert.Equal((byte)'.', message[5]);
            Assert.Equal((byte)'2', message[6]);
            Assert.Equal((byte)'9', message[7]);
            Assert.Equal(5, message[8]);
            Assert.Equal(48, message[9]);
            Assert.Equal(49, message[10]);
            Assert.Equal(52, message[11]);
            Assert.Equal(53, message[12]);
            Assert.Equal(3, message[13]);
        }

        [Fact]
        public void GenerateAndParseMessage()
        {
            var communication = new Communication(EmptyPort.Instance);
            var generatedMessage = communication.GenerateMessage(MessageType.M44LO, 32, 3.29, 42);

            var testPort = new ArraySegmentPort(generatedMessage);
            var testCommunication = new Communication(testPort);

            var receivedMessage = testCommunication.ReceiveMessage(fromPrinter: false);
            var doubleParameter = new Parameter<double>();
            var intParameter = new Parameter<int>();
            testCommunication.ParseDataMessage(receivedMessage.Data, MessageType.M44LO, new object[] { doubleParameter, intParameter }, fromPrinter: false);

            Assert.Equal(3.29, doubleParameter.Value);
            Assert.Equal(42, intParameter.Value);
        }

        private static Communication CreateParseCommunication(params byte[] bytes)
        {
            var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
            return new Communication(new ArraySegmentPort(bytes), encoding);
        }
    }
}
