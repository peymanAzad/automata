using System;

namespace formal_language_automata
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            string path;
            if (args.Length != 0)
            {
                path = args[0];
            }
            else
            {
                Console.WriteLine("Enter Machine Path");
                path = Console.ReadLine();
            }

            string command;
            do
            {
                command = Console.ReadLine();

                IMachine machine = new Machine(path);
                var dfa = Machine.Nfa2Dfa(machine);

                switch (command)
                {
                    case "printdfa":
                        Console.WriteLine(dfa.ToString());
                        break;
                    case "printgrammer":
                        Console.WriteLine(dfa.ToGrammer().ToString());
                        break;
                    case "printregx":
                        dfa.RemoveDStates();
                        Console.WriteLine(dfa.ToRegX());
                        break;
                }
            } while (command != "exit");

        }
    }
}
