using System;

namespace ExeTest
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0) {
                Console.Write($"<HTML><BODY><h1>");
                for (int i = 0; i < args.Length; i++)
                {
                    Console.Write(" "+args[i]);
                }
                Console.Write("</h1></BODY></HTML>");
            }
            else
                Console.WriteLine($"<HTML><BODY><h1>No Parameter</h1></BODY></HTML>");
        }
    }
}
