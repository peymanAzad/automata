using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formal_language_automata
{
    interface IState
    {
        string Name { get; set; }
        bool IsStart { get; set; }
        bool IsFinal { get; set; }
    }
}
