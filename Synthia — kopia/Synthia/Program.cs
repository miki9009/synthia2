using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthia
{
    class Program
    {
        static void Main()
        {
            Synthia synthia = new Synthia();
            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White; ;
                Console.Write("User: ");
                string text = Console.ReadLine();
                text = synthia.Response(text);
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Synthia: " + text + "\n");
            }
        }
    }
}
