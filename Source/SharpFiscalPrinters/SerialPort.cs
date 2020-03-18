using System;

namespace SharpFiscalPrinters
{
    public class SerialPort : IPort
    {
        public SerialPort(System.IO.Ports.SerialPort port)
        {
            Port = port;
        }

        public SerialPort(string portName)
        {
            Port = new System.IO.Ports.SerialPort(portName);
            Port.Open();
        }

        public SerialPort(string portName, int baudRate)
        {
            Port = new System.IO.Ports.SerialPort(portName, baudRate);
            Port.Open();
        }

        public SerialPort(string portName, int baudRate, System.IO.Ports.Parity parity)
        {
            Port = new System.IO.Ports.SerialPort(portName, baudRate, parity);
            Port.Open();
        }

        public SerialPort(string portName, int baudRate, System.IO.Ports.Parity parity, int dataBits)
        {
            Port = new System.IO.Ports.SerialPort(portName, baudRate, parity, dataBits);
            Port.Open();
        }

        public SerialPort(string portName, int baudRate, System.IO.Ports.Parity parity, int dataBits, System.IO.Ports.StopBits stopBits)
        {
            Port = new System.IO.Ports.SerialPort(portName, baudRate, parity, dataBits, stopBits);
            Port.Open();
        }

        public System.IO.Ports.SerialPort Port { get; private set; }

        public void Dispose()
        {
            Port?.Dispose();
        }

        public byte ReadByte()
        {
            int read = Port.ReadByte();

            if (read < 0)
                throw new Exception("There is no more data on Port.");
            return (byte)read;
        }

        public void WriteBytes(ArraySegment<byte> bytes)
        {
            Port.Write(bytes.Array, bytes.Offset, bytes.Count);
        }
    }
}
