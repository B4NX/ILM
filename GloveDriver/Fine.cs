using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;

namespace GloveDriver
{
    class Fine :IDisposable
    {
        SerialPort device;
        FileStream fs;
        public Fine()
        {
            device = new SerialPort("COM1", 9600);
        }

        public void CollectAndStore()
        {
            fs = File.Create("DATA.csv");
            device.Open();

            while (true)
            {
                fs.WriteByte((byte)device.ReadByte());
                fs.Write(System.Text.Encoding.ASCII.GetBytes(","), 0, System.Text.Encoding.ASCII.GetByteCount(","));
            }
        }

        void IDisposable.Dispose()
        {
            this.device.Dispose();
        }
    }
}
