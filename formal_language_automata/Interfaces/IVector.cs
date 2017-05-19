using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formal_language_automata
{
    interface IVector
    {
        IState State1 { get; set; }
        IState State2 { get; set; }
        string Parameter { get; set; }
    }
}
