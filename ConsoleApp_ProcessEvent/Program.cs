using ConsoleApp_ProcessEvent.helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp_ProcessEvent
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var c = new Counter();
            //c.ThreadholdReached += c_ThreadholdReached;

            c.StartRun();

            Console.ReadKey();
            c.StopRun();
            Console.ReadKey();
            c.StartRun();
            Console.ReadKey();

            Console.ReadKey();
            c.StopRun();
            Console.ReadKey();
            c.StartRun();
            Console.ReadKey();
        }

        static void c_ThreadholdReached(object sender, EventArgs e)
        {
            Console.WriteLine("The threshold was reached");
        }
    }
}
