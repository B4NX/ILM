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

            bool go = true;
            string input;
            while (go)
            {
                input = Console.ReadLine();

                foreach (byte b in board.ReadSignal())
                {
                    Console.WriteLine(b);
                }
                input = Console.ReadLine();
                board.SendSignal(input);
                if (input == "q")
                {
                    go = false;
                }
            }
            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
