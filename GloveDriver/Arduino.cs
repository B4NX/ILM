using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;

namespace GloveDriver
{
    class Arduino
    {
        SerialPort arduino;
        Thread readThread;
        Thread writeThread;
        public Arduino()
        {
            this.arduino = new SerialPort("COM3", 9600);
            arduino.Open();

            readBuffer = new Queue<byte[]>();
            writeBuffer = new Queue<byte[]>();

            readThread = new Thread(new ThreadStart(ReadAsync));
            readThread.Start();
            writeThread = new Thread(new ThreadStart(WriteAsync));
            writeThread.Start();
        }
        ~Arduino()
        {
            readThread.Abort();
            writeThread.Abort();
            arduino.Close();
        }

        public Queue<byte[]> readBuffer;
        public void ReadAsync()
        {
            while (true)
            {
                int bytesToRead = this.arduino.BytesToRead;
                if (bytesToRead == 0) continue;

                byte[] data = new byte[bytesToRead];
                arduino.Read(data, 0, bytesToRead);
                readBuffer.Enqueue(data);
                //This shouldn't need to be here
                Console.WriteLine(System.Text.Encoding.ASCII.GetString(data));
            }
        }

        public Queue<byte[]> writeBuffer;
        public void WriteAsync()
        {
            while (true)
            {
                if (writeBuffer.Count != 0)
                {
                    byte[] data = writeBuffer.Dequeue();
                    arduino.Write(data, 0, data.Length);
                }
            }
        }

        public void Explode()
        {

        }
    }
}
