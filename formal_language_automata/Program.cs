using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formal_language_automata
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            var path =
                @"c:\users\peyman!\documents\visual studio 2015\Projects\formal_language_automata\formal_language_automata\data.txt";
            var machine = new Machine(path);
            Console.WriteLine(machine.ToGrammer().ToString());
            Console.ReadKey();
        }
    }
}
