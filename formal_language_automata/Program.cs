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
            //var path =
            //    @"c:\users\peyman!\documents\visual studio 2015\Projects\formal_language_automata\formal_language_automata\data2.txt";
            var path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var index = path.IndexOf("formal_language_automata", StringComparison.Ordinal);
            path = path.Remove(index);
            path += @"formal_language_automata\formal_language_automata\data2.txt";

            IMachine machine = new Machine(path);
            machine = Machine.Nfa2Dfa(machine);
            Console.WriteLine(machine.ToRegX());
            Console.ReadKey();
        }
    }
}
