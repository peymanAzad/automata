using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formal_language_automata
{
    class State : IState
    {
        public string Name { get; set; }
        public bool IsStart { get; set; }
        public bool IsFinal { get; set; }
    }
}
