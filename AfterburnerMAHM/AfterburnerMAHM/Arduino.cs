using System;
using System.IO.Ports;

namespace AfterburnerMAHM
{
    class Arduino
    {
        private SerialPort port;
        public Arduino()
        {
            port = new SerialPort();
        }
        public bool Connect(String portName, int baudRate)
        {
            port.PortName = portName;
            port.BaudRate = baudRate;
            port.Open();
            return port.IsOpen;
        }
        public void close()
        {
            port.Close();
        }
        public void write(byte[] data, int offset, int length)
        {
            if (port.IsOpen)
            {
                port.Write(data, offset, length);
            }
        }
        public void write(String data)
        {
            if (port.IsOpen)
            {
                port.Write(data);
            }
        }
        public void writeln(String data)
        {
            if (port.IsOpen)
            {
                port.WriteLine(data);
            }
        }
    }
}
