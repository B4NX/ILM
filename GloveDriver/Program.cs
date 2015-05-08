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
            Arduino board = new Arduino();
            String ting = "aaaa";
            board.SendSignal("aaaa");

            foreach (byte b in board.ReadSignal())
            {
                Console.WriteLine(b);
            }

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
