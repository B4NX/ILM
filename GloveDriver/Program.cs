using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace GloveDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            DeviceManager board = null;
            bool go = true;
            try
            {
                board = new DeviceManager();
            }
            catch (System.IO.IOException e)
            {
                go = false;
                throw new Exception("No device was found on the specified COM port",e);
            }
            
            string input;
            while (go)
            {
                input = Console.ReadLine();

                //if (board.readBuffer.Count != 0)
                //{
                //    string message = ByteToString(board.readBuffer.Dequeue());
                //    Console.WriteLine(message);
                //}

                board.writeBuffer.Enqueue(StringToByte(input));
                if (input == "q")
                {
                    go = false;
                    board.Close();
                }
            }
            Console.WriteLine("Done");
        }

        public static byte[] StringToByte(string s)
        {
            return System.Text.Encoding.ASCII.GetBytes(s);
        }
        public static String ByteToString(byte[] b)
        {
            return System.Text.Encoding.ASCII.GetString(b);
        }
    }
}
