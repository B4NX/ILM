using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.IO;

namespace GloveDriver
{
    class Fine
    {
        SerialPort device;
        FileStream fs;
        public Fine()
        {
            device = new SerialPort("COM1", 9600);
        }
        ~Fine()
        {
            fs.Close();
            device.Close();
        }
        public void CollectAndStore()
        {
            fs = File.Create("DATA.csv");
            device.Open();

            while (true)
            {
                fs.WriteByte((byte)device.ReadByte());
            }
        }
    }
}
