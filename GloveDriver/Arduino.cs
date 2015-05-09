using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace GloveDriver
{
    class Arduino
    {
        SerialPort arduino;
        public Arduino()
        {
            this.arduino = new SerialPort("COM3", 9600);
            arduino.Open();
        }

        public byte[] ReadSignal()
        {
            int bytesToRead = this.arduino.BytesToRead;
            byte[] data = new byte[bytesToRead];
            arduino.Read(data, 0, bytesToRead);
            return data;
        }

        public void SendSignal(string data)
        {
            arduino.Write(data);
        }
        public void Explode()
        {

        }
    }
}
