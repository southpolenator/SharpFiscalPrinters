using SharpFiscalPrinters.FP550;
using System.Text;
using Xunit;

namespace SharpFiscalPrinters.Tests.FP550
{
    public class FP550Tests
    {
        [Fact]
        public void IsPrinterFiscalized()
        {
            var port = CreatePort(
                0x01, 0x31, 0x3c, 0x4a, 0x80, 0x80, 0x80, 0x85, 0x80, 0xba, 0x04, 0x80, 0x80, 0x80, 0x85, 0x80,
                0xba, 0x05, 0x30, 0x37, 0x33, 0x3e, 0x03);
            var fp550 = CreatePrinter(port, 60);
            var message = port.WrittenBytes;

            Assert.True(fp550.IsPrinterFiscalized());
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
        public void GetIdentificationNumberOfFiscalModule()
        {
            var port = CreatePort(
                0x01, 0x5C, 0x3D, 0x5a, 0x34, 0x2E, 0x32, 0x32, 0x53, 0x52, 0x20, 0x32, 0x30, 0x46, 0x45, 0x42,
                0x31, 0x33, 0x20, 0x31, 0x30, 0x30, 0x30, 0x2C, 0x38, 0x41, 0x33, 0x33, 0x2C, 0x31, 0x30, 0x31,
                0x30, 0x2C, 0x38, 0x2C, 0xC0, 0xa3, 0x30, 0x37, 0x34, 0x30, 0x33, 0x30, 0x2c, 0x30, 0x30, 0x30,
                0x30, 0x30, 0x30, 0x30, 0x30, 0x04, 0x80, 0x80, 0x80, 0x85, 0x80, 0xba, 0x05, 0x30, 0x3f, 0x30,
                0x30, 0x03);
            var fp550 = CreatePrinter(port, 61);
            var message = port.WrittenBytes;

            Assert.Equal("АЈ074030", fp550.GetIdentificationNumberOfFiscalModule());
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

        private static ExtendedFP550 CreatePrinter(IPort port, byte startingMessageId = Communication.MinimumMessageId)
        {
            var encoding = CodePagesEncodingProvider.Instance.GetEncoding(1251);
            return new ExtendedFP550(port, startingMessageId, encoding);
        }

        private static ArraySegmentPort CreatePort(params byte[] bytes)
        {
            return new ArraySegmentPort(bytes);
        }
    }
}
